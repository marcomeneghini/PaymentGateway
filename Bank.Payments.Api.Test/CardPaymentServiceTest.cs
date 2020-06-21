using System;
using System.Collections.Generic;
using Bank.Payments.Api.Domain;
using Bank.Payments.Api.Infrastructure;
using Bank.Payments.Api.Services;
using Xunit;
using Xunit.Extensions;

namespace Bank.Payments.Api.Test
{
    public class CardPaymentServiceTest
    {
        private string requestId1 = "aaaaaaaaaaaaaaa";

        [Theory]
        [MemberData(nameof(ValidPaymentsData))]
        public void CardPayment_ValidPayments(Card card, BankAccount bankAccount,TransactionStatus resulTransactionStatus)
        {
           
            var fakeCardRepository = new FakeCardRepository();
            var fakeBankAccountRepository = new FakeBankAccountRepository();
            var inMemPaymentRepository = new InMemoryPaymentRepository();
            var service = new CardPaymentService(fakeCardRepository, fakeBankAccountRepository, inMemPaymentRepository);
            CardPaymentResponse response = service.DoPayment(new CardPaymentRequest(card, bankAccount));

            Assert.Equal(resulTransactionStatus, response.TransactionStatus);
        }

        [Theory]
        [MemberData(nameof(InValidPaymentsData))]
        public void CardPayment_InValidPayments(Card card, BankAccount bankAccount, TransactionStatus resulTransactionStatus)
        {

            var fakeCardRepository = new FakeCardRepository();
            var fakeBankAccountRepository = new FakeBankAccountRepository();
            var inMemPaymentRepository = new InMemoryPaymentRepository();
            var service = new CardPaymentService(fakeCardRepository, fakeBankAccountRepository, inMemPaymentRepository);
            CardPaymentResponse response = service.DoPayment(new CardPaymentRequest(card, bankAccount));

            Assert.Equal(resulTransactionStatus, response.TransactionStatus);
        }

        [Fact]
        public void CardPayment_DoublePayment()
        {
            // arrange
            var fakeCardRepository = new FakeCardRepository();
            var fakeBankAccountRepository = new FakeBankAccountRepository();
            var inMemPaymentRepository = new InMemoryPaymentRepository();
            // act
            var service = new CardPaymentService(fakeCardRepository, fakeBankAccountRepository, inMemPaymentRepository);
            CardPaymentResponse response = service.DoPayment(new CardPaymentRequest(requestId1, FakeCardRepository.GenerateCard_JohnDoe(), FakeBankAccountRepository.GenerateBankAccount_Amazon()));
            service = new CardPaymentService(new FakeCardRepository(), new FakeBankAccountRepository(), inMemPaymentRepository);
            // act-assert
            Assert.Throws<RequestAlreadyProcessedException>(() => service.DoPayment(new CardPaymentRequest(requestId1, FakeCardRepository.GenerateCard_JohnDoe(), FakeBankAccountRepository.GenerateBankAccount_Amazon())));
           
        }


        public static IEnumerable<object[]> ValidPaymentsData
        {
            get
            {
                // Or this could read from a file. :)
                return new[]
                {
                    new object[] { FakeCardRepository.GenerateCard_JohnDoe(), FakeBankAccountRepository.GenerateBankAccount_Amazon(),TransactionStatus.Succeeded },
                    new object[] { FakeCardRepository.GenerateCard_JaneDoe(), FakeBankAccountRepository.GenerateBankAccount_Amazon(),TransactionStatus.Succeeded },
                    new object[] {  FakeCardRepository.GenerateCard_JohnDoe(), FakeBankAccountRepository.GenerateBankAccount_Apple(),TransactionStatus.Succeeded },
                    new object[] {  FakeCardRepository.GenerateCard_JaneDoe(), FakeBankAccountRepository.GenerateBankAccount_Apple(),TransactionStatus.Succeeded },
                 
                };
            }
        }

        public static IEnumerable<object[]> InValidPaymentsData
        {
            get
            {
                // Or this could read from a file. :)
                return new[]
                {
                    new object[] { FakeCardRepository.GenerateCard_JaneDoe(), FakeBankAccountRepository.GenerateBankAccount_NotPresent(),TransactionStatus.Declined },
                    new object[] { FakeCardRepository.GenerateCard_NotPresent(), FakeBankAccountRepository.GenerateBankAccount_Apple(), TransactionStatus.Declined },
                  

                };
            }
        }

        private CardPaymentRequest Create_Card1_PaymentRequest()
        {
            return new CardPaymentRequest()
            {
                Amount = 10,
                CVV = "000",
                CardHolderName = "Holder Card1 FullName",
                CardNumber = "1234 1234 1234 1234",
                Currency = "GBP",
                MerchantAccountNumber = "12345678",
                MerchantSortCode = "112233",
                MonthExpiryDate = 1,
                YearExpiryDate = 2021,
                RequestId = requestId1
            };
        }
    }
}
