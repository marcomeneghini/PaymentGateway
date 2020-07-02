using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Domain.Entities
{
    public class Consts
    {
        public const string UKNOWN_ERROR_CODE = "??";

        public const string PG_UNEXPECTED_ERROR_CODE     = "XX";
        public const string PG_VALIDATION_ERROR_CODE     = "01";
        public const string PG_DUPLICATE_REQUEST_CODE    = "02";
        public const string PG_MERCHANT_INVALID_CODE     = "03";
        public const string PG_MERCHANT_NOT_PRESENT_CODE = "04";


        public const string PROC_UNEXPECTED_ERROR_CODE               = "YY";
        public const string PROC_VALIDATION_ERROR_CODE               = "10";
        public const string PROC_PAYMENTSTATUS_NOTFOUND_ERRORCODE    = "11";
    }
}
