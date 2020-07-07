using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Infrastructure
{
    public class InMemoryPaymentRepository: IPaymentRepository
    {
        private ConcurrentDictionary<string, PaymentStatus> paymentStatuses =
            new ConcurrentDictionary<string, PaymentStatus>();
        public async Task<PaymentStatus> GetPaymentStatus(string requestId)
        {
            return await Task.Run(() =>
            {
                if (paymentStatuses.TryGetValue(requestId, out var paymentStatus))
                {
                    return paymentStatus;
                }

                return null;
            });
        }

        public async Task AddPaymentStatus(PaymentStatus paymentStatus)
        {
             await Task.Run(() =>
             {
                 paymentStatuses.TryAdd(paymentStatus.RequestId, paymentStatus);
                 return Task.CompletedTask;
             });

        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {

            await Task.Run(() =>
            {
                if (paymentStatuses.TryGetValue(paymentStatus.RequestId, out var existingPaymentStatus))
                {
                    paymentStatuses.TryUpdate(paymentStatus.RequestId, paymentStatus, existingPaymentStatus);
                }
                return Task.CompletedTask;
            });
        }
    }
}
