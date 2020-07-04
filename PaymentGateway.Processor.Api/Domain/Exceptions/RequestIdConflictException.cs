using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain.Exceptions
{
    public class RequestIdConflictException:Exception
    {
        public string RequestId { get; private set; }

        public RequestIdConflictException():base()
        {
            
        }

        public RequestIdConflictException(string requestId ):base("RequestId already processed")
        {
            RequestId = requestId;
        }
    }
}
