using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.SharedLib.Messages;
using System.Threading.Tasks;
namespace PaymentGateway.SharedLib.EventBroker
{
    public interface IEventBrokerSubscriber
    {
   
        void Subscribe(string exchangeName,string queueName);
        event EventHandler<EncryptedMessageEventArgs> OnMessage;
        void Unsubscribe();

        //void Subscribe<T, TH>()
        //    where T : EncryptedMessage
        //    where TH : IIntegrationEventHandler<T>;

        //void SubscribeDynamic<TH>(string eventName)
        //    where TH : IDynamicIntegrationEventHandler;

        //void UnsubscribeDynamic<TH>(string eventName)
        //    where TH : IDynamicIntegrationEventHandler;

        //void Unsubscribe<T, TH>()
        //    where TH : IIntegrationEventHandler<T>
        //    where T : EncryptedMessage;
    }
}
