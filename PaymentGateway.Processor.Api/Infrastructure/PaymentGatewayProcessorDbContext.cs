using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.Processor.Api.Infrastructure
{
    
    public class PaymentGatewayProcessorDbContext : DbContext
    {
        public PaymentGatewayProcessorDbContext(DbContextOptions<PaymentGatewayProcessorDbContext> options)
            : base(options)
        {
            // LoadMerchants();
            Database.EnsureCreated();

        }

        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
    }
}
