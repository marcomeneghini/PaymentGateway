using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class ChannelConsumer:IChannelConsumer
    {
        private readonly ChannelReader<EncryptedMessage> _reader;
        private readonly ILogger<ChannelConsumer> _logger;

        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IBankPaymentProxy _bankPaymentProxy;
        private readonly ICipherService _cipherService;

        private static readonly Random Random = new Random();

        public ChannelConsumer(
            ChannelReader<EncryptedMessage> reader, 
            ILogger<ChannelConsumer> logger,
            IPaymentStatusRepository paymentStatusRepository,
            IBankPaymentProxy bankPaymentProxy,
            ICipherService cipherService )
        {
            _reader = reader;
            _logger = logger;
            _paymentStatusRepository = paymentStatusRepository;
            _bankPaymentProxy = bankPaymentProxy;
            _cipherService = cipherService;
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
                    
                    // save the payment status
                    //var paymentStatus = new PaymentStatus();
                    //paymentStatus.Status = PaymentStatusEnum.Scheduled;
                    //paymentStatus.RequestId = decryptedMessage.RequestId;
                    //paymentStatus.PaymentId = decryptedMessage.PaymentRequestId;
                    //await _paymentStatusRepository.AddPaymentStatus(paymentStatus);
                    var paymentStatus= await _paymentStatusRepository.GetPaymentStatus(decryptedMessage.PaymentRequestId);
                    if (paymentStatus==null)
                    {
                        // TODO:Exception, something wrong with the persistent store
                        throw new Exception("paymentStatus==null , something wrong with the persistent store");
                    }
                    // perform the call 
                    var bankPaymentResponse =await _bankPaymentProxy.CreatePaymentAsync(new CardPaymentRequest
                    {
                        Amount = decryptedMessage.Amount,
                        CVV =  decryptedMessage.CVV,
                        CardNumber = decryptedMessage.CardNumber,
                        CardHolderName = decryptedMessage.CardHolderName,
                        MonthExpiryDate = decryptedMessage.MonthExpiryDate,
                        YearExpiryDate = decryptedMessage.YearExpiryDate,
                        Currency = decryptedMessage.Currency,
                        MerchantSortCode = decryptedMessage.MerchantSortCode,
                        MerchantAccountNumber = decryptedMessage.MerchantAccountNumber,
                        RequestId = decryptedMessage.RequestId
                    });
                    if (bankPaymentResponse==null)
                    {
                        throw new Exception("bankPaymentResponse==null , something wrong with the communication");
                        // TODO: Exception, something wrong with the communication
                    }
                    if (bankPaymentResponse.TransactionStatus== TransactionStatus.Declined)
                    {
                        paymentStatus.Status = PaymentStatusEnum.Error;
                    }
                    else
                    {
                        paymentStatus.Status = PaymentStatusEnum.Completed;
                    }

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
