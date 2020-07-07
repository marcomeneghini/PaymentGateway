using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Domain.Exceptions;

namespace PaymentGateway.Processor.Api.Infrastructure
{
    public class EfPaymentStatusRepository : IPaymentStatusRepository
    {
        private readonly PaymentGatewayProcessorDbContext _dbContext;

        public EfPaymentStatusRepository(PaymentGatewayProcessorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      
        public async Task<PaymentStatus> GetPaymentStatus(Guid paymentId)
        {
            return await _dbContext.PaymentStatuses.FirstOrDefaultAsync(x => x.PaymentId == paymentId);
        }

        public async Task AddPaymentStatus(PaymentStatus paymentStatus)
        {
            await _dbContext.AddAsync<PaymentStatus>(paymentStatus);
            await _dbContext.SaveChangesAsync();

        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            var existingPaymentStatus = await _dbContext.PaymentStatuses.FirstOrDefaultAsync(x => x.RequestId == paymentStatus.RequestId);
            if (existingPaymentStatus != null)
            {
                existingPaymentStatus.Status = paymentStatus.Status;
            }

            await _dbContext.SaveChangesAsync();
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
