using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.SharedLib.EventBroker
{
    public class EncryptedMessageEventArgs : EventArgs
    {
        public EncryptedMessageEventArgs(EncryptedMessage message)
        {
            this.Message = message;
        }

        public EncryptedMessage Message { get; }
    }
}
