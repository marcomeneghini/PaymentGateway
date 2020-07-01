using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Entities
{
    public class PaymentStatus
    {
        [Key]
        public Guid PaymentId { get; set; }

        public string RequestId { get; set; }

        public PaymentStatusEnum Status { get; set; }
    }
}
