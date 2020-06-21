using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Payments.Api.Domain;

namespace Bank.Payments.Api.Services
{
    public interface ICardPaymentService
    {
        CardPaymentResponse DoPayment(CardPaymentRequest request);
    }
}
