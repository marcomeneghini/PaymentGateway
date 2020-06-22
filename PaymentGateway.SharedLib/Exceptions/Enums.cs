using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Exceptions
{
    public enum MessageDecryptionErrorReason
    {
        NoContentTypeName,
        EmptyBody,
        WrongCipher
    }
}
