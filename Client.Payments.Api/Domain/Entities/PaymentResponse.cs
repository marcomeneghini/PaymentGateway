using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Domain.Entities
{
    public class PaymentResponse
    {
        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public string RequestId { get; set; }

        public Guid PaymentRequestId { get; set; }
    }
}
