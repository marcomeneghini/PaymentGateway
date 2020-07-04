using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Domain.Entities
{
    public static class Consts
    {
        public const string UNEXPECTED_ERROR_CODE = "99";
        public const string VALIDATION_ERROR_CODE = "01";
        public const string BANK_REQUESTID_DUPLICATED_ERRORCODE = "02";
        public const string BANK_PAYMENT_WRONGDETAILS_ERRORCODE = "03";
        public const string PAYMENTSTATUS_NOTFOUND_ERRORCODE = "04";
    }
}
