using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Domain.Entities
{
    public class PaymentStatusResponse
    {
        // Common fields
        public string ErrorCode { get; set; }

        public string Message { get; set; }

        //------------------

        public string Status { get; set; }

        public Guid TransactionId { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }

}
