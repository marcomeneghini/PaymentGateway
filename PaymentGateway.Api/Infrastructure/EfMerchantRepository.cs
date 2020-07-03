using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Infrastructure
{
    public class EfMerchantRepository: IMerchantRepository
    {
        private readonly PaymentGatewayDbContext _dbContext;

        public EfMerchantRepository(PaymentGatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static Merchant CreateMerchant_Amazon()
        {
            return new Merchant()
            {
                Id = Guid.Parse("53D92C77-3C0E-447C-ABC5-0AF6CF829A22"),
                AccountNumber = "AmazonAccountNumber",
                Denomination = "Amazon",
                IsValid = true,
                SortCode = "AAMMZZ"
            };

        }

        public static Merchant CreateMerchant_Apple()
        {
            return new Merchant()
            {
                Id = Guid.Parse("11112C77-3C0E-447C-ABC5-0AF6CF821111"),
                AccountNumber = "AppleAccountNumber",
                Denomination = "Apple",
                IsValid = false,
                SortCode = "AAPPLL"
            };

        }

        public async Task<Merchant> GetMerchantById(Guid merchantId)
        {
         
            return await _dbContext.Merchants.FirstOrDefaultAsync(x => x.Id == merchantId);
        }
    }
}
