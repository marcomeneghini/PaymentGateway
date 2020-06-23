using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class PaymentDetailsException:Exception
    {
        public PaymentDetailsException():base()
        {
                
        }

        public PaymentDetailsException(string message):base(message)
        {
            
        }
    }
}
