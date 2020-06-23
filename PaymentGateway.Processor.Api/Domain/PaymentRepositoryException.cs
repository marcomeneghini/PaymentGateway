using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class PaymentRepositoryException:Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public PaymentRepositoryException():base()
        {
                
        }

        public PaymentRepositoryException(string message, HttpStatusCode httpStatusCode= HttpStatusCode.InternalServerError) :base(message)
        {
            
        }
    }
}
