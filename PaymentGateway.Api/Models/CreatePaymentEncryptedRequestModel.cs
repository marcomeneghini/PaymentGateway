using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Models
{
    public class CreatePaymentEncryptedRequestModel
    {
        public string RequestId { get; set; }

        public string Payload { get; set; }

    }
}
