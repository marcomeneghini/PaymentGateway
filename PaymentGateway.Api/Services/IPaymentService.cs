using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Services
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request);
    }
}
