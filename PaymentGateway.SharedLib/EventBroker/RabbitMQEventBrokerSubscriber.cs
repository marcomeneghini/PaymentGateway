using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using PaymentGateway.SharedLib.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentGateway.SharedLib.EventBroker
{
    public class RabbitMQEventBrokerSubscriber:IEventBrokerSubscriber,IDisposable
    {
        private readonly IRabbitMQPersistentConnection _connection;
        private IModel _channel;
        private QueueDeclareOk _queue;

        public RabbitMQEventBrokerSubscriber(IRabbitMQPersistentConnection connection)
        {
            _connection = connection;
        }
        public void Subscribe(string exchangeName, string routingKey, string queueName)
        {
            InitChannel(exchangeName, routingKey, queueName);
            InitSubscribe();
        }
        
        public event AsyncEventHandler<EncryptedMessageEventArgs> OnMessage;
        public void Unsubscribe()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received -= Consumer_Received; ;
            //
            _channel.Close();
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        #region Private Members

        private void InitChannel(string exchangeName,string routingKey, string queueName)
        {
            _channel?.Dispose();
            _connection.TryConnect();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct,true);

           
            _queue = _channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(_queue.QueueName, exchangeName, routingKey, null);

            _channel.CallbackException += _channel_CallbackException;

            //TODO: VERIFY this code
            //_channel.CallbackException += (sender, ea) =>
            //{
            //    InitChannel(exchangeName, routingKey, queueName);
            //    InitSubscribe();
            //};
        }

        private void _channel_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            var a = e;
        }

        private void InitSubscribe()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += Consumer_Received; ;

            _channel.BasicConsume(queue: _queue.QueueName, autoAck: false, consumer: consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var message = JsonSerializer.Deserialize<EncryptedMessage>(body);
            await OnMessage?.Invoke(this, new EncryptedMessageEventArgs(message));

        }
        #endregion
    }
}
