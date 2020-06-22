using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Exceptions;
using PaymentGateway.SharedLib.Messages;
using RabbitMQ.Client;

namespace PaymentGateway.SharedLib.EventBroker
{
    public class RabbitMqEventBrokerPublisher:IEventBrokerPublisher
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _hostname;

        public RabbitMqEventBrokerPublisher(string username, string password, string hostname)
        {
            _username = username;
            _password = password;
            _hostname = hostname;
        }
        public void PublishEncryptedMessage(EncryptedMessage encryptedMessage)
        {
            try
            {
                var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
                {
                    UserName = _username,
                    Password = _password,
                    HostName = _hostname
                };
                var connection = connectionFactory.CreateConnection();

                var model = connection.CreateModel();

                // Create Exchange, this is Idempotent so i don't bother to check the existence
                model.ExchangeDeclare(encryptedMessage.TopicName, ExchangeType.Direct);

                //publish 
                var properties = model.CreateBasicProperties();
                properties.Persistent = false;

                var serializedMessage = JsonConvert.SerializeObject(encryptedMessage);

                byte[] messagebuffer = Encoding.Default.GetBytes(serializedMessage);

                model.BasicPublish(encryptedMessage.TopicName, "directexchange_key", properties, messagebuffer);
            }
            catch (Exception e)
            {
                throw new EventBrokerException(e.Message);
            }
        }

    }
}
