using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Payments.Api.Domain.Entities;

namespace Client.Payments.Api.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> DoPaymentAsync(Payment payment);
    }
}
