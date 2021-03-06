﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Models
{
    public class PaymentStatusModel
    {
        public Guid PaymentId { get; set; }
        public string RequestId { get; set; }

        public string Status { get; set; }

        public string TransactionId { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
