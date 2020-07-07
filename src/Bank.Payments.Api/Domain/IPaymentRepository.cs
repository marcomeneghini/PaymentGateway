using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Domain
{
    public interface IPaymentRepository
    {
        PaymentStatus GetPaymentStatus(string requestId);
        void AddPaymentStatus(PaymentStatus paymentStatus);

        void UpdatePaymentStatus(PaymentStatus paymentStatus);
    }
}
