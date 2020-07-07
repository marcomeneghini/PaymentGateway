using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain.Exceptions
{
    public class PaymentNotFoundException:Exception
    {

        public HttpStatusCode HttpStatusCode { get; private set; }
        public PaymentNotFoundException():base()
        {
            
        }

        public PaymentNotFoundException(string message,HttpStatusCode httpStatusCode= HttpStatusCode.NotFound):base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
