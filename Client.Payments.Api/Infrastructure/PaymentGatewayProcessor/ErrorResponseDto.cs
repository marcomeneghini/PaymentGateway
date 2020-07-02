using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Infrastructure.PaymentGatewayProcessor
{
    public class ErrorResponseDto
    {
        public string ErrorCode { get; set; }

        public string ReferenceCode { get; set; }
        public string RequestId { get; set; }

        public string Message { get; set; }
    }
}
