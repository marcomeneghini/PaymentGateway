using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class BankPaymentDetailsException:Exception
    {
        public BankPaymentDetailsException():base()
        {
                
        }

        public BankPaymentDetailsException(string message):base(message)
        {
            
        }
    }
}
