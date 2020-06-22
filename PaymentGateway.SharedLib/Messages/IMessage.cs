using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Messages
{
    public  interface IMessage
    {
        string Serialize();
    }
}
