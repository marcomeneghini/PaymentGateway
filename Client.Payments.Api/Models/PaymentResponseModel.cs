﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Models
{
    public class PaymentResponseModel
    {
        public string ReferenceCode { get; set; }

        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public string RequestId { get; set; }

        public Guid PaymentRequestId { get; set; }

        // from the PROC

        public string Status { get; set; }

        public Guid TransactionId { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

