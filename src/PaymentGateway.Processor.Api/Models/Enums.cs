using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Models
{
    public enum PaymentStatusEnum
    {
        Scheduled,
        Completed,
        Error
    }
}
