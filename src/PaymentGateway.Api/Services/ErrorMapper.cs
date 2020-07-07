using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Services
{
    public class ErrorMapper:IErrorMapper
    {
        public string GetMessage(string errCode)
        {
            if (errCode == Consts.MERCHANT_NOT_PRESENT_CODE)
                return "Merchant not present";
            if (errCode == Consts.DUPLICATE_REQUEST_CODE)
                return "Duplicate request";
            if (errCode == Consts.MERCHANT_INVALID_CODE)
                return "Merchant not valid";
            return string.Empty;
        }

        public string GetMessage(string errCode, string parameter)
        {
            if (errCode == Consts.MERCHANT_NOT_PRESENT_CODE)
                return $"Merchant not present. MerchantId:{parameter}";
            if (errCode == Consts.DUPLICATE_REQUEST_CODE)
                return $"Duplicate request. ReqeustId:{parameter}";
            if (errCode == Consts.MERCHANT_INVALID_CODE)
                return $"Merchant not valid. MerchantId:{parameter}";
            return string.Empty;
        }
    }
}
