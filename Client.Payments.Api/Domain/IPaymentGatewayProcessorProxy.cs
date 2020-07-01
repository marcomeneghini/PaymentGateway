using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Payments.Api.Domain.Entities;

namespace Client.Payments.Api.Domain
{
    public interface IPaymentGatewayProcessorProxy
    {
        Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid paymentStatusGuid);
    }
}
