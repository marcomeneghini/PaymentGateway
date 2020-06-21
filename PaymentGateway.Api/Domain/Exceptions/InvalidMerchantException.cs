using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Exceptions
{
    public class InvalidMerchantException:Exception
    {
        public Guid MerchantId { get; private set; }
        public InvalidMerchantReason InvalidMerchantReason { get; private set; }

        public InvalidMerchantException(Guid merchantId,InvalidMerchantReason invalidMerchantReason)
        {
            MerchantId = merchantId;
            InvalidMerchantReason = invalidMerchantReason;
        }
    }
}
