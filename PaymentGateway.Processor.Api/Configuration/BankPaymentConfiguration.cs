using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Configuration
{
    public class BankPaymentConfiguration
    {
        public const string SectionName = "BankPayments";
        public string BaseAddress { get; set; }
    }
}
