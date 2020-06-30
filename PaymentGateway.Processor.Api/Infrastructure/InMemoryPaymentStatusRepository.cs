using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Domain.Exceptions;

namespace PaymentGateway.Processor.Api.Infrastructure
{
    public class InMemoryPaymentStatusRepository: IPaymentStatusRepository
    {
        private ConcurrentDictionary<Guid, PaymentStatus> paymentStatuses =
            new ConcurrentDictionary<Guid, PaymentStatus>();

        public InMemoryPaymentStatusRepository()
        {
            var scheduled = Create_Scheduled_PaymentStatus();
            paymentStatuses.TryAdd(scheduled.PaymentId, scheduled);

            var completed = Create_Completed_PaymentStatus();
            paymentStatuses.TryAdd(completed.PaymentId, completed);

            var error = Create_Error_PaymentStatus();
            paymentStatuses.TryAdd(error.PaymentId, error);
        }
        public async Task<PaymentStatus> GetPaymentStatus(Guid paymentId)
        {
            var paymentStatus= await Task.Run(() =>
            {
                if (paymentStatuses.TryGetValue(paymentId, out var paymentStatus))
                {
                    return paymentStatus;
                }
                return null;
            });
           
            return paymentStatus;
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


        #region Static Methods

        public static PaymentStatus Create_Scheduled_PaymentStatus()
        {
            return new PaymentStatus()
            {
                PaymentId = Guid.Parse("55577777-4444-447C-ABC5-0AF6CF829A22"),
                RequestId =  Guid.NewGuid().ToString(),
                Status = PaymentStatusEnum.Scheduled.ToString()
            };

        }

        public static PaymentStatus Create_Completed_PaymentStatus()
        {
            return new PaymentStatus()
            {
                PaymentId = Guid.Parse("66677777-4444-447C-ABC5-0AF6CF829A22"),
                RequestId = Guid.NewGuid().ToString(),
                Status = PaymentStatusEnum.Completed.ToString(),
                UpdatedAt = DateTimeOffset.Now.AddSeconds(3),
                TransactionId = "transaction1"
            };
        }


        public static PaymentStatus Create_Error_PaymentStatus()
        {
            return new PaymentStatus()
            {
                PaymentId = Guid.Parse("77777777-4444-447C-ABC5-0AF6CF829A22"),
                RequestId = Guid.NewGuid().ToString(),
                Status = PaymentStatusEnum.Error.ToString(),
                UpdatedAt = DateTimeOffset.Now.AddSeconds(3),
                TransactionId = "transaction2"
            };
        }

        #endregion
    }
}
