using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.Processor.Api.IntegrationTests
{
    public static class Helper
    {
        public static CardPaymentResponse CreateFake_Succeeded_CardPaymentResponse(string requestId)
        {
            return new CardPaymentResponse()
            {
                RequestId = requestId,
                TransactionStatus  = TransactionStatus.Succeeded.ToString(),
                TransactionId=Guid.NewGuid().ToString()
            };
        }

        
    }
}
