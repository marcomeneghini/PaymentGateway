using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Proxies
{
    public class CardPaymentResponseDto
    {
        public string RequestId { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public string Message { get; set; }
    }
}
