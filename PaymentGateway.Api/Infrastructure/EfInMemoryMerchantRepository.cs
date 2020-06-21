using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Infrastructure
{
    public class EfInMemoryMerchantRepository: IMerchantRepository
    {
        private readonly PaymentGatewayDbContext _dbContext;

        public EfInMemoryMerchantRepository(PaymentGatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static Merchant CreateMerchant_Amazon()
        {
            return new Merchant()
            {
                Id =Guid.Parse("53D92C77-3C0E-447C-ABC5-0AF6CF829A22"),
                AccountNumber = "11111111111111",
                Denomination = "Amazon",
                IsValid = true,
                SortCode = "000000"
            };

        }

        public static Merchant CreateMerchant_Apple()
        {
            return new Merchant()
            {
                Id = Guid.Parse("11112C77-3C0E-447C-ABC5-0AF6CF821111"),
                AccountNumber = "2222222222222",
                Denomination = "Apple",
                IsValid = true,
                SortCode = "123456"
            };

        }

        public async Task<Merchant> GetMerchantById(Guid merchantId)
        {
            //var merchants = await _dbContext.Merchants.ToListAsync();

            //var merchants2 =  _dbContext.GetMerchants();
            //return  merchants2.FirstOrDefault(x => x.Id == merchantId);
            return await _dbContext.Merchants.FirstOrDefaultAsync(x => x.Id == merchantId);
        }
    }
}
