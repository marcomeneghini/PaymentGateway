using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
     

        public EventBrokerBackgroundWorker(
            IEventBrokerSubscriber eventBrokerSubscriber,
            IChannelProducer producer,
            IChannelConsumer consumer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<EventBrokerBackgroundWorker> logger)
        {
            _eventBrokerSubscriber = eventBrokerSubscriber ?? throw new ArgumentNullException(nameof(eventBrokerSubscriber));
            _eventBrokerSubscriber.OnMessage += _eventBrokerSubscriber_OnMessage1; 
           
            _producer = producer;
            _consumer = consumer;
            _serviceScopeFactory = serviceScopeFactory;
        
            _logger = logger;
        }

        private async Task _eventBrokerSubscriber_OnMessage1(object sender, EncryptedMessageEventArgs e)
        {
            EncryptedMessage message = e.Message;
            message.ProcessedAt = DateTimeOffset.Now;
            _logger.LogInformation($"message type: {e.Message.ContentTypeName} pushedAt:{e.Message.PushedAt} processedAt:{message.ProcessedAt}");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var paymentStatusRepository = scope.ServiceProvider.GetRequiredService<IPaymentStatusRepository>();
                await _producer.PublishAsync(paymentStatusRepository, message);
                
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _eventBrokerSubscriber.Subscribe(EventBrokerConsts.PAYMENT_REQUEST_EXCHANGE_NAME, EventBrokerConsts.PAYMENT_REQUEST_ROUTINGKEY, "Processor.Api");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var paymentStatusRepository = scope.ServiceProvider.GetRequiredService<IPaymentStatusRepository>();
                    await _consumer.BeginConsumeAsync(paymentStatusRepository,stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

    }
}
