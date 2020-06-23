using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class BankNotAvailableException:Exception
    {
        public BankNotAvailableException():base()
        {
                
        }

        public BankNotAvailableException(string message):base(message)
        {
            
        }
    }
}
