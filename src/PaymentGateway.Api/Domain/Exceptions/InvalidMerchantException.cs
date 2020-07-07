using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Domain.Exceptions
{
    public class InvalidMerchantException:Exception
    {
        public Guid MerchantId { get; private set; }
        public InvalidMerchantReason InvalidMerchantReason { get; private set; }

        public HttpStatusCode HttpStatusCode { get; private set; }
        public InvalidMerchantException(Guid merchantId,InvalidMerchantReason invalidMerchantReason, HttpStatusCode statusCode= HttpStatusCode.NotFound) 
            :base($"merchantId:{merchantId}, reason:{invalidMerchantReason}")
        {
            MerchantId = merchantId;
            InvalidMerchantReason = invalidMerchantReason;
            HttpStatusCode = statusCode;
        }


        public override string ToString()
        {
            return $"MerchantId:{MerchantId}, InvalidMerchantReason:{InvalidMerchantReason}";
        }
    }
}
