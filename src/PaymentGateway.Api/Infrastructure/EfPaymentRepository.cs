using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.Infrastructure
{
    public class EfPaymentRepository:IPaymentRepository
    {
        private readonly PaymentGatewayDbContext _dbContext;

        public EfPaymentRepository(PaymentGatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PaymentStatus> GetPaymentStatus(string requestId)
        {
           return await _dbContext.PaymentStatuses.FirstOrDefaultAsync(x => x.RequestId == requestId);
           
        }

        public async Task AddPaymentStatus(PaymentStatus paymentStatus)
        {
            await _dbContext.AddAsync<PaymentStatus>(paymentStatus);
            await  _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatus(PaymentStatus paymentStatus)
        {
            var existingPaymentStatus = await _dbContext.PaymentStatuses.FirstOrDefaultAsync(x => x.RequestId == paymentStatus.RequestId);
            if (existingPaymentStatus!=null)
            {
                existingPaymentStatus.Status = paymentStatus.Status;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
