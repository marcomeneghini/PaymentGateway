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
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Domain.Exceptions;
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
            _logger.LogInformation($"ChannelConsumer > starting");

            try
            {
                await foreach (var message in _reader.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        _logger.LogInformation($"ChannelConsumer > Received message {message.Id} : {message.TopicName}");

                        // decrypt the message
                        var decryptedMessage = message.GetMessage<PaymentRequestMessage>(_cipherService);
                        PaymentStatus paymentStatus = null;
                        try
                        {
                            paymentStatus = await _paymentStatusRepository.GetPaymentStatus(decryptedMessage.PaymentRequestId);
                        }
                        catch (PaymentRepositoryException e)
                        {
                            _logger.LogError($"PaymentRepositoryException:{e.Message}");
                        }

                        CardPaymentResponse bankPaymentResponse = null;
                       
                        var request = _mapper.Map<CardPaymentRequest>(decryptedMessage);


                        var policy = Policy.Handle<SocketException>()
                            .Or<BankNotAvailableException>()
                            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (ex, time) =>
                                {
                                    _logger.LogWarning(ex,
                                        "Bank Payment Service now available after {TimeOut}s ({ExceptionMessage})",
                                        $"{time.TotalSeconds:n1}", ex.Message);
                                }
                            );
                        policy.Execute(() =>
                        {
                            // perform the call 
                            try
                            {
                                bankPaymentResponse = _bankPaymentProxy.CreatePaymentAsync(request).Result;
                            }
                            catch (AggregateException e)
                            {
                                var flatExceptions = e.Flatten().InnerExceptions;

                                foreach (var agrEx in flatExceptions)
                                {
                                    _logger.LogError($"{agrEx.GetType().Name}:{e.Message}");
                                }
                            }
                        });


                        if (bankPaymentResponse == null)
                        {
                            paymentStatus.Status = PaymentStatusEnum.Error.ToString();
                            _logger.LogError($"Received Null from the server. RequestId:{decryptedMessage.RequestId}");
                        }
                        else
                        {
                            paymentStatus.Status = bankPaymentResponse?.TransactionStatus == TransactionStatus.Declined.ToString() ? PaymentStatusEnum.Error.ToString() : PaymentStatusEnum.Completed.ToString();
                            paymentStatus.TransactionId = bankPaymentResponse.TransactionId;
                        }
                        

                        await _paymentStatusRepository.UpdatePaymentStatus(paymentStatus);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical($"Unexpected exception:{e.Message}");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"ChannelConsumer > forced stop");
            }

            _logger.LogInformation($"ChannelConsumer > shutting down");
        }
    }
}
