using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Exceptions
{
    public class RequestAlreadyProcessedException : Exception
    {
        public PaymentStatusEnum Status { get; private set; }

        public string RequestId { get; private set; }
        public RequestAlreadyProcessedException(PaymentStatusEnum status, string requestId)
        {
            Status = status;
            RequestId = requestId;
        }

    }
}
