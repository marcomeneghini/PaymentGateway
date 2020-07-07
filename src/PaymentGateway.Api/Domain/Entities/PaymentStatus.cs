using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Entities
{
    public class PaymentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PaymentId { get; set; }

        public string RequestId { get; set; }

        public PaymentStatusEnum Status { get; set; }
    }
}
