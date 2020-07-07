using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class ChannelProducer:IChannelProducer
    {

        private readonly ChannelWriter<EncryptedMessage> _writer;
        //private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly ICipherService _cipherService;
        private readonly ILogger<ChannelProducer> _logger;

        public ChannelProducer(
            ChannelWriter<EncryptedMessage> writer,
           
            ICipherService cipherService,
            ILogger<ChannelProducer> logger)
        {
            _writer = writer;
            _cipherService = cipherService;
            _logger = logger;
        }

        public async Task PublishAsync(
           IPaymentStatusRepository paymentStatusRepository, 
            EncryptedMessage message, 
            CancellationToken cancellationToken = default)
        {
            // decrypt the message
            var decryptedMessage = message.GetMessage<PaymentRequestMessage>(_cipherService);

            // save the payment status
            var paymentStatus = new PaymentStatus
            {
                Status = PaymentStatusEnum.Scheduled.ToString(),
                RequestId = decryptedMessage.RequestId,
                PaymentId = decryptedMessage.PaymentRequestId
            };
            try
            {
                await paymentStatusRepository.AddPaymentStatus(paymentStatus);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Probable duplicated Id: {e.Message}");
            }
            
            await _writer.WriteAsync(message, cancellationToken);
            
            _logger.LogInformation($"Producer > published message {message.Id} '{message.TopicName}'");
        }

    }
}
