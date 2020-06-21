using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain
{
    public interface IPaymentRepository
    {
        Task<PaymentStatus> GetPaymentStatus(string requestId);
        Task AddPaymentStatus(PaymentStatus paymentStatus);

        Task UpdatePaymentStatus(PaymentStatus paymentStatus);
    }
}
