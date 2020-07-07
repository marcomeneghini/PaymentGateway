using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Exceptions;
using PaymentGateway.SharedLib.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Polly;
using Polly.Retry;

namespace PaymentGateway.SharedLib.EventBroker
{
    public class RabbitMQEventBrokerPublisher:IEventBrokerPublisher
    {
        private readonly ILogger<RabbitMQEventBrokerPublisher> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly int _retrycount;
        const string BROKER_NAME = "paymentgateway_event_bus";

        public RabbitMQEventBrokerPublisher(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQEventBrokerPublisher> logger,
            int retrycount)
        {
            _logger = logger;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _retrycount = retrycount;
        }
        public void PublishEncryptedMessage(EncryptedMessage encryptedMessage)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retrycount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", encryptedMessage.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = encryptedMessage.ContentTypeName;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", encryptedMessage.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {

                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", encryptedMessage.Id);

                //channel.ExchangeDeclare(exchange: encryptedMessage.TopicName, type: "direct",true);
                channel.ExchangeDeclare(exchange: encryptedMessage.TopicName, type: ExchangeType.Direct, true);
                var message = JsonConvert.SerializeObject(encryptedMessage);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", encryptedMessage.Id);

                    channel.BasicPublish(
                        exchange: encryptedMessage.TopicName,
                        routingKey: encryptedMessage.RoutingKey,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }


        }

    }
}
