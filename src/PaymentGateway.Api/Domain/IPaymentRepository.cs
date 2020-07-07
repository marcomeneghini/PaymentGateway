using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Domain
{
    public interface IPaymentRepository
    {
        Task<PaymentStatus> GetPaymentStatus(string requestId);
        Task AddPaymentStatus(PaymentStatus paymentStatus);

        Task UpdatePaymentStatus(PaymentStatus paymentStatus);
    }
}
