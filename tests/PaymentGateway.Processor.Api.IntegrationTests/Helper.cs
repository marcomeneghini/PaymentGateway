using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.Processor.Api.IntegrationTests
{
    public static class Helper
    {
        public static PaymentResult CreateFake_Succeeded_CardPaymentResponse(string requestId)
        {
            return new PaymentResult()
            {
                RequestId = requestId,
                TransactionStatus  = TransactionStatus.Succeeded.ToString(),
                TransactionId=Guid.NewGuid().ToString()
            };
        }

        
    }
}
