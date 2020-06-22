﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public enum PaymentStatusEnum
    {
        Scheduled,
        Completed,
        Error

    }

    public enum TransactionStatus
    {
        Declined,
        Succeeded
    }
}
