using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Domain;

namespace PaymentGateway.Api.Infrastructure
{
   
    public class PaymentGatewayDbContext : DbContext
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options)
            : base(options)
        {
        }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<PaymentStatus> PaymentStatuses { get; set; }

    }
}
