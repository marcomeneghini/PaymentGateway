using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Payments.Api.Domain;

namespace Bank.Payments.Api.Infrastructure
{
    public class FakeBankAccountRepository:IBankAccountRepository
    {
        public List<BankAccount> GetAllBankAccounts()
        {
            return  new List<BankAccount>()
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
                AccountNumber = "11111111111111",
                SortCode = "000000"
            };
        }

        public static BankAccount GenerateBankAccount_Apple()
        {
            return new BankAccount()
            {
                AccountHolder = "Apple",
                AccountNumber = "2222222222222",
                SortCode = "123456"
            };
        }

        public static BankAccount GenerateBankAccount_NotPresent()
        {
            return new BankAccount()
            {
                AccountHolder = "Mark and Spencer",
                AccountNumber = "5555555555",
                SortCode = "778899"
            };
        }
    }
}
