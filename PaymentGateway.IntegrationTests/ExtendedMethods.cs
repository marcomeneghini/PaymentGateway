using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.IntegrationTests
{
    public static class ExtendedMethods
    {
        public static Card GetCard(this CardPaymentRequest cardPaymentRequest)
        {
            return new Card()
                {
                    CardHolderName = cardPaymentRequest.CardHolderName,
                    MonthExpiryDate = cardPaymentRequest.MonthExpiryDate,
                    YearExpiryDate = cardPaymentRequest.YearExpiryDate,
                    CardNumber = cardPaymentRequest.CardNumber
                };
        }

        public static BankAccount GetBankAccount(this CardPaymentRequest cardPaymentRequest)
        {
            return new BankAccount()
            {
                AccountNumber = cardPaymentRequest.MerchantAccountNumber,
                SortCode = cardPaymentRequest.MerchantSortCode
            };
        }
    }
}
