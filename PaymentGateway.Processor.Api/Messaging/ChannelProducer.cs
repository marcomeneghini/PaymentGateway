using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class ChannelProducer:IChannelProducer
    {

        private readonly ChannelWriter<EncryptedMessage> _writer;
        private readonly ILogger<ChannelProducer> _logger;

        public ChannelProducer(ChannelWriter<EncryptedMessage> writer, ILogger<ChannelProducer> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task PublishAsync(EncryptedMessage message, CancellationToken cancellationToken = default)
        {
            await _writer.WriteAsync(message, cancellationToken);
            _writer.Complete();
            _logger.LogInformation($"Producer > published message {message.Id} '{message.TopicName}'");
        }

    }
}
