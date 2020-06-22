using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.SharedLib.EventBroker;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public class EventBrokerBackgroundWorker: BackgroundService
    {
        private readonly IEventBrokerSubscriber _eventBrokerSubscriber;
        private readonly ILogger<EventBrokerBackgroundWorker> _logger;

        private readonly IChannelProducer _producer;
        private readonly IChannelConsumer _consumer;

        public EventBrokerBackgroundWorker(
            IEventBrokerSubscriber eventBrokerSubscriber,
            IChannelProducer producer,
            IChannelConsumer consumer,
            ILogger<EventBrokerBackgroundWorker> logger)
        {
            _eventBrokerSubscriber = eventBrokerSubscriber ?? throw new ArgumentNullException(nameof(eventBrokerSubscriber));
            _eventBrokerSubscriber.OnMessage += _eventBrokerSubscriber_OnMessage; ;
           
            _producer = producer;
            _consumer = consumer;
            _logger = logger;
        }

       

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: add an instance variable to this name "Processor.Api"
            _eventBrokerSubscriber.Subscribe(EventBrokerConsts.PAYMENT_REQUEST_EXCHANGE_NAME, "Processor.Api");
            await _consumer.BeginConsumeAsync(stoppingToken);
        }

        private void _eventBrokerSubscriber_OnMessage(object sender, EncryptedMessageEventArgs e)
        {
            EncryptedMessage message = e.Message;
            message.ProcessedAt=DateTimeOffset.Now;
            _logger.LogInformation($"message type: {e.Message.ContentTypeName} pushedAt:{e.Message.PushedAt} processedAt:{message.ProcessedAt}");

             _producer.PublishAsync(message);
        }
    }
}
