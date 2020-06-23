using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.SharedLib.Messages;
using System.Threading.Tasks;
namespace PaymentGateway.SharedLib.EventBroker
{
    public interface IEventBrokerSubscriber
    {

        void Subscribe(string exchangeName, string routingKey, string queueName);
        event EventHandler<EncryptedMessageEventArgs> OnMessage;
        void Unsubscribe();

       
    }
}
