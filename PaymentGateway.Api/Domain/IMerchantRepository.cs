using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain
{
    public interface IMerchantRepository
    {
        Task<Merchant> GetMerchantById(Guid merchantId);
    }
}
