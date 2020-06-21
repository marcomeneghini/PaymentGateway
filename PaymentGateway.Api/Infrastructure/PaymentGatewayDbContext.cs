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
            LoadMerchants();
        }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<PaymentStatus> PaymentStatuses { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Merchant>().HasData(EfInMemoryMerchantRepository.CreateMerchant_Amazon());

        //    modelBuilder.Entity<Merchant>().HasData(EfInMemoryMerchantRepository.CreateMerchant_Amazon());

        //}

        public void LoadMerchants()
        {
            
            Merchants.Add(EfInMemoryMerchantRepository.CreateMerchant_Amazon());
           
            Merchants.Add(EfInMemoryMerchantRepository.CreateMerchant_Apple());
        }

        public List<Merchant> GetMerchants()
        {
            return Merchants.Local.ToList<Merchant>();
        }

    }
}
