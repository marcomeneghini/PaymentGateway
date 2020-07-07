using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.Exceptions
{
    public class EventBrokerException:Exception
    {
        public EventBrokerException(string message):base(message)
        {
            
        }
    }
}
