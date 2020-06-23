using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            return await Task.Run(() =>
            {
                if (paymentStatuses.TryGetValue(paymentId, out var paymentStatus))
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
                try
                {
                    paymentStatuses.TryAdd(paymentStatus.PaymentId, paymentStatus);
                }
                catch (Exception e)
                {
                    throw new PaymentRepositoryException(e.Message);
                }
                return Task.CompletedTask;
            });


        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (paymentStatuses.TryGetValue(paymentStatus.PaymentId, out var existingPaymentStatus))
                    {
                        paymentStatuses.TryUpdate(paymentStatus.PaymentId, paymentStatus, existingPaymentStatus);
                    }
                }
                catch (Exception e)
                {
                    throw new PaymentRepositoryException(e.Message);
                }

                return Task.CompletedTask;
            });
            
        }
    }
}
