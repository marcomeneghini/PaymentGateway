using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Api.Infrastructure.PaymentGatewayProcessor
{
    public class PaymentGatewayProcessorProxy: IPaymentGatewayProcessorProxy
    {
        private readonly HttpClient _httpClient;

        public PaymentGatewayProcessorProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid paymentStatusGuid)
        {
            throw new NotImplementedException();
        }
    }
}
