using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.IntegrationTests
{
    public static class Helper
    {
        public static PaymentResult CreateFake_Succeeded_CardPaymentResponse()
        {
            return new PaymentResult()
            {
                RequestId = "",
                TransactionStatus  = TransactionStatus.Succeeded.ToString(),
                TransactionId=Guid.NewGuid().ToString()
            };
        }

        

        public static IBankPaymentProxy CreateBankPaymentProxyMock()
        {

            var mockBankPaymentProxy = new Mock<IBankPaymentProxy>();
            mockBankPaymentProxy.Setup(m => m.CreatePaymentAsync(It.IsAny<CardPayment>()))
                .ReturnsAsync((CardPayment request) =>
                {
                    var cards = GetAllCards();
                    var currentCard = request.GetCard();
                    if (!cards.Contains(currentCard))
                        return new PaymentResult { TransactionStatus = TransactionStatus.Declined.ToString(), Message = "Wrong card details", RequestId = request.RequestId };
                    var bankAccounts = GetAllBankAccounts();
                    if (!bankAccounts.Contains(request.GetBankAccount()))
                        return new PaymentResult { TransactionStatus = TransactionStatus.Declined.ToString(), Message = "Wrong bank account details", RequestId = request.RequestId };
                    return new PaymentResult { TransactionStatus = TransactionStatus.Succeeded.ToString(), RequestId = request.RequestId, TransactionId = Guid.NewGuid().ToString()};
                });
            return mockBankPaymentProxy.Object;
        }

        private static List<Card> GetAllCards()
        {
            return new List<Card>( )
            {
                GenerateCard_JohnDoe(),
                GenerateCard_JaneDoe()
            };
        }
        public static Card GenerateCard_JohnDoe()
        {
            return new Card()
            {
                CardHolderName = "John Doe",
                CardNumber = "1298 1111 1111 1111",
                MonthExpiryDate = 1,
                YearExpiryDate = 2021,
                CVV = "111"
            };
        }

        public static Card GenerateCard_JaneDoe()
        {
            return new Card()
            {
                CardHolderName = "Jane Doe",
                CardNumber = "1298 2222 2222 2222",
                MonthExpiryDate = 2,
                YearExpiryDate = 2022,
                CVV = "222"
            };
        }

        public static List<BankAccount> GetAllBankAccounts()
        {
            return new List<BankAccount>()
            {
                GenerateBankAccount_Amazon(),
                GenerateBankAccount_Apple()
            };
        }

        public static BankAccount GenerateBankAccount_Amazon()
        {
            return new BankAccount()
            {
                AccountHolder = "Amazon",
                AccountNumber = "AmazonAccountNumber",
                SortCode = "AAMMZZ"

            };
        }

        public static BankAccount GenerateBankAccount_Apple()
        {
            return new BankAccount()
            {
                AccountHolder = "Apple",
                AccountNumber = "AppleAccountNumber",
                SortCode = "AAPPLL"
            };
        }
    }
}
