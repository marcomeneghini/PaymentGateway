using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Exceptions
{
    public class RequestAlreadyProcessedException : Exception
    {
        public PaymentStatusEnum Status { get; private set; }

        public string RequestId { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }
        public RequestAlreadyProcessedException(PaymentStatusEnum status, string requestId, HttpStatusCode statusCode = HttpStatusCode.Conflict)
            : base($"status:{status}, requestId:{requestId}")
        {
            Status = status;
            RequestId = requestId;
            HttpStatusCode = statusCode;
        }

        public override string ToString()
        {
            return $"PaymentStatus:{Status}, RequestId:{RequestId}";
        }
    }
}
