﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Models
{
    public class CardPaymentResponseModel
    {
        public string RequestId { get; set; }

        public string TransactionStatus { get; set; }

        public string Message { get; set; }

        public string TransactionId { get; set; }
    }
}
