using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Api.Services
{
    public class PaymentService:IPaymentService
    {
        private readonly IPaymentGatewayProcessorProxy _paymentGatewayProcessorProxy;
        private readonly IPaymentGatewayProxy _paymentGatewayProxy;
        private readonly Guid merchantIdGuid;

        public PaymentService(
            IPaymentGatewayProcessorProxy paymentGatewayProcessorProxy,
            IPaymentGatewayProxy paymentGatewayProxy,
            IConfiguration configuration)
        {
            _paymentGatewayProcessorProxy = paymentGatewayProcessorProxy;
            _paymentGatewayProxy = paymentGatewayProxy;
            if (!Guid.TryParse(configuration["MerchantId"], out merchantIdGuid))
                throw  new ArgumentNullException("MerchantId");
        }

        public async Task<PaymentResponse> DoPaymentAsync(Payment payment)
        {
            // send the payment request through _paymentGatewayProxy
            payment.MerchantId = merchantIdGuid;
            payment.RequestId = Guid.NewGuid().ToString();
            var response =await  _paymentGatewayProxy.CreatePaymentAsync(payment);
            if (response!=null && string.IsNullOrEmpty(response.ErrorCode) )
            {
                for (int i = 0; i < 3; i++)
                {
                    Task.Delay(1000);
                    var paymentStatus = await _paymentGatewayProcessorProxy.GetPaymentStatusAsync(response.PaymentRequestId);
                    if (paymentStatus!=null && string.IsNullOrEmpty(paymentStatus.ErrorCode))
                    {
                        response.CreatedAt = paymentStatus.CreatedAt;
                        response.UpdatedAt = paymentStatus.UpdatedAt;
                        response.TransactionId = paymentStatus.TransactionId;
                        response.Status = paymentStatus.Status;
                        if (response.Status!="Scheduled")
                        {
                           break; 
                        }
                    }
                }
            }

            return response;
        }
    }
}
