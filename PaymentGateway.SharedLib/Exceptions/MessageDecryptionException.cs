using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Exceptions
{
    public class MessageDecryptionException:Exception
    {
        public MessageDecryptionErrorReason MessageDecryptionErrorReason { get; private set; }

        public MessageDecryptionException(MessageDecryptionErrorReason  messageDecryptionErrorReason):base()
        {
            MessageDecryptionErrorReason = messageDecryptionErrorReason;
        }

        public MessageDecryptionException(MessageDecryptionErrorReason messageDecryptionErrorReason,string message):base(message)
        {
            MessageDecryptionErrorReason = messageDecryptionErrorReason;
        }
    }
}
