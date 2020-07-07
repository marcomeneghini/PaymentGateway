using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Domain
{
    public class BankAccount:IEquatable<BankAccount>
    {
        public string AccountHolder { get; set; }
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }

        public bool Equals(BankAccount other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SortCode == other.SortCode && AccountNumber == other.AccountNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BankAccount) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SortCode, AccountNumber);
        }
    }
}
