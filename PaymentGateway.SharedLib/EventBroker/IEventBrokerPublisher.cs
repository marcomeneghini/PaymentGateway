using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.SharedLib.EventBroker
{
    public interface IEventBrokerPublisher
    {
        void PublishEncryptedMessage(EncryptedMessage encryptedMessage);

    }
}
