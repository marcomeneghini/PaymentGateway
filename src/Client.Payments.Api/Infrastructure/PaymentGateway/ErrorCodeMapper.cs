using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  Domain= Client.Payments.Api.Domain;
namespace Client.Payments.Api.Infrastructure.PaymentGateway
{
    public static class ErrorCodeMapper
    {
        public static string MapErrorCodeToDomain(string paymentGatewayErrorCode)
        {
            if (paymentGatewayErrorCode == Consts.MERCHANT_INVALID_CODE)
                return Domain.Entities.Consts.PG_MERCHANT_INVALID_CODE;

            if (paymentGatewayErrorCode == Consts.DUPLICATE_REQUEST_CODE)
                return Domain.Entities.Consts.PG_DUPLICATE_REQUEST_CODE;

            if (paymentGatewayErrorCode == Consts.MERCHANT_NOT_PRESENT_CODE)
                return Domain.Entities.Consts.PG_MERCHANT_NOT_PRESENT_CODE;

            if (paymentGatewayErrorCode == Consts.VALIDATION_ERROR_CODE)
                return Domain.Entities.Consts.PG_VALIDATION_ERROR_CODE;

            return Domain.Entities.Consts.UKNOWN_ERROR_CODE;
        }
    }
}
