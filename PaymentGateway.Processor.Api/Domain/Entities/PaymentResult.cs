﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain.Entities
{
    public class PaymentResult
    {
        public string RequestId { get; set; }

        public string TransactionStatus { get; set; }

        public string Message { get; set; }

        public string TransactionId { get; set; }

        public string ErrorCode { get; set; }
    }
}
