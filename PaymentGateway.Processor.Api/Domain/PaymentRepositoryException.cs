using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class PaymentRepositoryException:Exception
    {
        public PaymentRepositoryException():base()
        {
                
        }

        public PaymentRepositoryException(string message):base(message)
        {
            
        }
    }
}
