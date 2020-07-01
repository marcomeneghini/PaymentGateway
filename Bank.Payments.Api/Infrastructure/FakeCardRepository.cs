using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bank.Payments.Api.Domain;

namespace Bank.Payments.Api.Infrastructure
{
    public class FakeCardRepository:ICardRepository
    {
        public List<Card> GetAllCards()
        {
            return new List<Card>()
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
                YearExpiryDate = 2021
            };
        }

        public static Card GenerateCard_JaneDoe()
        {
            return new Card()
            {
                CardHolderName = "Jane Doe",
                CardNumber = "1298 2222 2222 2222",
                MonthExpiryDate = 2,
                YearExpiryDate = 2022
            };
        }

        public static Card GenerateCard_NotPresent()
        {
            return new Card()
            {
                CardHolderName = "sssss",
                CardNumber = "4444 6666 1234 1234",
                MonthExpiryDate = 1,
                YearExpiryDate = 2021,
            };
        }
    }
}
