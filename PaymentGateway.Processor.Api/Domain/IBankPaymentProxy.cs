using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain
{
    public interface IBankPaymentProxy
    {
        Task<CardPaymentResponse> CreatePaymentAsync(CardPaymentRequest request);
    }
}
