using System;
using Bank.Payments.Api.Domain;
using Bank.Payments.Api.Services;
using Xunit;

namespace Bank.Payments.Api.Test
{
    public class CardPaymentServiceTest
    {
        [Fact]
        public void CardPayment_Card1Succeeded()
        {
            var service = new CardPaymentService();
            CardPaymentResponse response = service.DoPayment( new CardPaymentRequest());

            Assert.Equal(TransactionStatus.Succeeded,response.TransactionStatus);
        }
    }
}
