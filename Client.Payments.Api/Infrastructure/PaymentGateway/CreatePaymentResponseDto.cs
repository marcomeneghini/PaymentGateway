using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Infrastructure.PaymentGateway
{
    public class CreatePaymentResponseDto
    {
        public string RequestId { get; set; }

        public Guid PaymentRequestId { get; set; }
    }
}
