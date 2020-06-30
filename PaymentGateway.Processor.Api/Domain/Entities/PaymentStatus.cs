using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain.Entities
{
    public class PaymentStatus
    {
        private string _status;
        private string _requestId;

        public PaymentStatus()
        {
            CreatedAt= DateTimeOffset.Now;
            UpdatedAt = CreatedAt;
        }

        [Key] 
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; }
        
        public string RequestId
        {
            get => _requestId;
            set
            {
                _requestId = value;
                UpdatedAt = DateTimeOffset.Now;
            }
        }

        public string Status
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
