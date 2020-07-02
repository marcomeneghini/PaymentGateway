using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  Domain= Client.Payments.Api.Domain;
namespace Client.Payments.Api.Infrastructure.PaymentGatewayProcessor
{
    public static class ErrorCodeMapper
    {
        public static string MapErrorCodeToDomain(string paymentGatewayErrorCode)
        {
         

            if (paymentGatewayErrorCode == Consts.PAYMENTSTATUS_NOTFOUND_ERRORCODE)
                return Domain.Entities.Consts.PROC_PAYMENTSTATUS_NOTFOUND_ERRORCODE;

            if (paymentGatewayErrorCode == Consts.VALIDATION_ERROR_CODE)
                return Domain.Entities.Consts.PROC_VALIDATION_ERROR_CODE;

            if (paymentGatewayErrorCode == Consts.UNEXPECTED_ERROR_CODE)
                return Domain.Entities.Consts.PROC_UNEXPECTED_ERROR_CODE;

            return Domain.Entities.Consts.UKNOWN_ERROR_CODE;
        }
    }
}
