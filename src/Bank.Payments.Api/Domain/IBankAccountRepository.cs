using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Domain
{
    public interface IBankAccountRepository
    {
        List<BankAccount> GetAllBankAccounts();
    }
}
