using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Models
{
    public class CreatePaymentResponseModel
    {
        public string RequestId { get; set; }

        public Guid PaymentRequestId { get; set; }
    }
}
