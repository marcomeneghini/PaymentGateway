using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Infrastructure
{
    public class InMemoryMerchantRepository: IMerchantRepository
    {
        private ConcurrentDictionary<Guid, Merchant> merchants =
            new ConcurrentDictionary<Guid, Merchant>();

        public InMemoryMerchantRepository()
        {
            var amazon = CreateMerchant_Amazon();
            merchants.TryAdd(amazon.Id, amazon);

            var apple = CreateMerchant_Apple();
            merchants.TryAdd(apple.Id, apple);
        }
        public async Task<Merchant> GetMerchantById(Guid merchantId)
        {
            return await Task.Run(() =>
            {
                if (merchants.TryGetValue(merchantId, out var merchant))
                {
                    return merchant;
                }

                return null;
            });
        }

        public static Merchant CreateMerchant_Amazon()
        {
            return new Merchant()
            {
                Id = Guid.Parse("53D92C77-3C0E-447C-ABC5-0AF6CF829A22"),
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
                IsValid = false,
                SortCode = "123456"
            };

        }
    }
}
