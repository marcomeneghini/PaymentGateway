using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.Processor.Api.Infrastructure
{
    public class InMemoryPaymentStatusRepository: IPaymentStatusRepository
    {
        private ConcurrentDictionary<Guid, PaymentStatus> paymentStatuses =
            new ConcurrentDictionary<Guid, PaymentStatus>();
        public async Task<PaymentStatus> GetPaymentStatus(Guid paymentId)
        {
            if (paymentStatuses.TryGetValue(paymentId, out var paymentStatus))
            {
                return paymentStatus;
            }

            return null;
        }

        public async Task AddPaymentStatus(PaymentStatus paymentStatus)
        {
            paymentStatuses.TryAdd(paymentStatus.PaymentId, paymentStatus);
        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            if (paymentStatuses.TryGetValue(paymentStatus.PaymentId, out var existingPaymentStatus))
            {
                paymentStatuses.TryUpdate(paymentStatus.PaymentId, paymentStatus, existingPaymentStatus);
            }
        }
    }
}
