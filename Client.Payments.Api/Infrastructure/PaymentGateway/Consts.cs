using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Infrastructure.PaymentGateway
{
    public class Consts
    {
        public const string UNEXPECTED_ERROR_CODE = "99";
        public const string VALIDATION_ERROR_CODE = "01";
        public const string DUPLICATE_REQUEST_CODE = "02";
        public const string MERCHANT_INVALID_CODE = "03";
        public const string MERCHANT_NOT_PRESENT_CODE = "04";
    }
}
