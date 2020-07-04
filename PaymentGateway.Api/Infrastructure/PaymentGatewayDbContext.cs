using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Infrastructure
{
   
    public class PaymentGatewayDbContext : DbContext
    {
        public PaymentGatewayDbContext(DbContextOptions<PaymentGatewayDbContext> options)
            : base(options)
        {
           // LoadMerchants();
            Database.EnsureCreated();
          
        }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<PaymentStatus> PaymentStatuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var amazon = EfMerchantRepository.CreateMerchant_Amazon();
            var apple = EfMerchantRepository.CreateMerchant_Apple();

            modelBuilder.Entity<Merchant>()
                .HasData(amazon);
            modelBuilder.Entity<Merchant>()
                .HasData(apple);

        }

        public void LoadMerchants()
        {
            
            Merchants.Add(EfMerchantRepository.CreateMerchant_Amazon());
           
            Merchants.Add(EfMerchantRepository.CreateMerchant_Apple());
        }

        public List<Merchant> GetMerchants()
        {
            return Merchants.Local.ToList<Merchant>();
        }

    }
}
