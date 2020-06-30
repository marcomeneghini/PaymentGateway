using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Services
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> CreatePayment(Payment payment);
    }
}
