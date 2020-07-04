using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.Processor.Api.Domain
{
    public interface IBankPaymentProxy
    {
        Task<PaymentResult> CreatePaymentAsync(CardPayment request);
    }
}
