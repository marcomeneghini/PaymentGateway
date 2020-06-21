using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Models
{
    public enum TransactionStatus : int
    {
        Declined = 0,
        Succeeded = 1
    }
}
