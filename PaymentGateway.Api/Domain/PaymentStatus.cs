using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain
{
    public class PaymentStatus
    {
        public Guid PaymentId { get; set; }
        public string RequestId { get; set; }

        public PaymentStatusEnum Status { get; set; }
    }
}
