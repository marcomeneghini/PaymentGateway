using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Domain
{
    public class PaymentStatus
    {
        public string RequestId { get; set; }

        public PaymentStatusEnum Status { get; set; }    
    }
}
