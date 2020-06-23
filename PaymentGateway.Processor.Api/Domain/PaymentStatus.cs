using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public class PaymentStatus
    {
        private PaymentStatusEnum _status;
        private string _requestId;

        public PaymentStatus()
        {
            CreatedAt=DateTimeOffset.Now;
        }

        [Key]
        public Guid PaymentId { get; set; }

        public string RequestId
        {
            get => _requestId;
            set
            {
                _requestId = value;
                UpdatedAt = DateTimeOffset.Now;
            }
        }

        public PaymentStatusEnum Status
        {
            get => _status;
            set
            {
                _status = value;
                UpdatedAt = DateTimeOffset.Now;
            } 
        }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
