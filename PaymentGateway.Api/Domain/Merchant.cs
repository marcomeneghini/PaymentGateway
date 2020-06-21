using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PaymentGateway.Api.Domain
{
    public class Merchant
    {
        [Key]
        public Guid Id { get; set; }

        public string Denomination { get; set; }

        public string AccountNumber { get; set; }

        public string SortCode { get; set; }

        public bool IsValid { get; set; }
    }
}
