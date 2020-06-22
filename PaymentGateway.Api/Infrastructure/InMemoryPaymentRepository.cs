using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Infrastructure
{
    public class InMemoryPaymentRepository: IPaymentRepository
    {
        private ConcurrentDictionary<string, PaymentStatus> paymentStatuses =
            new ConcurrentDictionary<string, PaymentStatus>();
        public async Task<PaymentStatus> GetPaymentStatus(string requestId)
        {
            if (paymentStatuses.TryGetValue(requestId, out var paymentStatus))
            {
                return paymentStatus;
            }

            return null;
        }

        public async Task AddPaymentStatus(PaymentStatus paymentStatus)
        {
            paymentStatuses.TryAdd(paymentStatus.RequestId, paymentStatus);
        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            if (paymentStatuses.TryGetValue(paymentStatus.RequestId, out var existingPaymentStatus))
            {
                paymentStatuses.TryUpdate(paymentStatus.RequestId, paymentStatus, existingPaymentStatus);
            }
        }
    }
}
