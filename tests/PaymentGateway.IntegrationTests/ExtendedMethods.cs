using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.IntegrationTests
{
    public static class ExtendedMethods
    {
        public static Card GetCard(this CardPayment cardPaymentRequest)
        {
            return new Card()
                {
                    CardHolderName = cardPaymentRequest.CardHolderName,
                    MonthExpiryDate = cardPaymentRequest.MonthExpiryDate,
                    YearExpiryDate = cardPaymentRequest.YearExpiryDate,
                    CardNumber = cardPaymentRequest.CardNumber,
                    CVV =  cardPaymentRequest.CVV

            };
        }

        public static BankAccount GetBankAccount(this CardPayment cardPaymentRequest)
        {
            return new BankAccount()
            {
                AccountNumber = cardPaymentRequest.MerchantAccountNumber,
                SortCode = cardPaymentRequest.MerchantSortCode
            };
        }
    }
}
