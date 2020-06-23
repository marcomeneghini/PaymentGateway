using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.SharedLib.Messages;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace PaymentGateway.SharedLib.EventBroker
{
    public interface IEventBrokerSubscriber
    {

        void Subscribe(string exchangeName, string routingKey, string queueName);

        // this event doesnt seem to work because it is SYNC
        //event EventHandler<EncryptedMessageEventArgs> OnMessage;

        // must use the RabbytMQ  AsyncEventHandler !!!!
        event AsyncEventHandler<EncryptedMessageEventArgs> OnMessage;

        void Unsubscribe();

       
    }
}
