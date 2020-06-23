using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class ChannelProducer:IChannelProducer
    {

        private readonly ChannelWriter<EncryptedMessage> _writer;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly ICipherService _cipherService;
        private readonly ILogger<ChannelProducer> _logger;

        public ChannelProducer(
            ChannelWriter<EncryptedMessage> writer,
            IPaymentStatusRepository paymentStatusRepository,
            ICipherService cipherService,
            ILogger<ChannelProducer> logger)
        {
            _writer = writer;
            _paymentStatusRepository = paymentStatusRepository;
            _cipherService = cipherService;
            _logger = logger;
        }

        public async Task PublishAsync(EncryptedMessage message, CancellationToken cancellationToken = default)
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
            await _paymentStatusRepository.AddPaymentStatus(paymentStatus);
            await _writer.WriteAsync(message, cancellationToken);
            
            _logger.LogInformation($"Producer > published message {message.Id} '{message.TopicName}'");
        }

    }
}
