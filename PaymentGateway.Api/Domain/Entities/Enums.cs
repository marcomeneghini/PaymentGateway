using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Entities
{
    public enum InvalidMerchantReason
    {
        NotPresent,
        Invalid
    }

    public enum PaymentStatusEnum
    {
        Scheduled,
        Completed,
        Error
        
    }
}
