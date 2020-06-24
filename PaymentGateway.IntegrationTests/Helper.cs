using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.IntegrationTests
{
    public static class Helper
    {
        public static CardPaymentResponse CreateFake_Succeeded_CardPaymentResponse()
        {
            return new CardPaymentResponse()
            {
                RequestId = "",
                TransactionStatus  = TransactionStatus.Succeeded.ToString(),
                TransactionId=Guid.NewGuid().ToString()
            };
        }

        
    }
}
