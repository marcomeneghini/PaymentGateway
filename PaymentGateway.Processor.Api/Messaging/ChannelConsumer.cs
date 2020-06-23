using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;
using Polly;
using Polly.Retry;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class ChannelConsumer:IChannelConsumer
    {
        private readonly ChannelReader<EncryptedMessage> _reader;
        private readonly ILogger<ChannelConsumer> _logger;

        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IBankPaymentProxy _bankPaymentProxy;
        private readonly ICipherService _cipherService;
        private readonly IMapper _mapper;

        private static readonly Random Random = new Random();

        public ChannelConsumer(
            ChannelReader<EncryptedMessage> reader, 
            ILogger<ChannelConsumer> logger,
            IPaymentStatusRepository paymentStatusRepository,
            IBankPaymentProxy bankPaymentProxy,
            ICipherService cipherService,
            IMapper mapper)
        {
            _reader = reader;
            _logger = logger;
            _paymentStatusRepository = paymentStatusRepository;
            _bankPaymentProxy = bankPaymentProxy;
            _cipherService = cipherService;
            _mapper = mapper;
        }

        public async Task BeginConsumeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Consumer > starting");

            try
            {
                await foreach (var message in _reader.ReadAllAsync(cancellationToken))
                {
                    _logger.LogInformation($"CONSUMER > Received message {message.Id} : {message.TopicName}");

                    // decrypt the message
                    var decryptedMessage = message.GetMessage<PaymentRequestMessage>(_cipherService);
                  
                    var paymentStatus= await _paymentStatusRepository.GetPaymentStatus(decryptedMessage.PaymentRequestId);

                    CardPaymentResponse bankPaymentResponse = null;
                    try
                    {
                        var request = _mapper.Map<CardPaymentRequest>(decryptedMessage);


                        var policy = Policy.Handle<SocketException>()
                            .Or<BankNotAvailableException>()
                            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                                {
                                    _logger.LogWarning(ex, "Bank Payment Service now available after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                                }
                            );
                        policy.Execute(() =>
                        {
                            // perform the call 
                            bankPaymentResponse =  _bankPaymentProxy.CreatePaymentAsync(request).Result;
                        });
                    }
                    catch (Exception)
                    {
                        paymentStatus.Status = PaymentStatusEnum.Error;
                        await _paymentStatusRepository.UpdatePaymentStatus(paymentStatus);
                        throw;
                    }

                    if (bankPaymentResponse == null)
                        throw new BankNotAvailableException("Received Null from the server");
                    
                    paymentStatus.Status = bankPaymentResponse?.TransactionStatus== TransactionStatus.Declined ? PaymentStatusEnum.Error : PaymentStatusEnum.Completed;

                    await _paymentStatusRepository.UpdatePaymentStatus(paymentStatus);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"Consumer > forced stop");
            }

            _logger.LogInformation($"Consumer > shutting down");
        }
    }
}
