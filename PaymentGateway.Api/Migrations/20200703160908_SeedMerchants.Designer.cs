﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentGateway.Api.Infrastructure;

namespace PaymentGateway.Api.Migrations
{
    [DbContext(typeof(PaymentGatewayDbContext))]
    [Migration("20200703160908_SeedMerchants")]
    partial class SeedMerchants
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PaymentGateway.Api.Domain.Entities.Merchant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Denomination")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("SortCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Merchants");

                    b.HasData(
                        new
                        {
                            Id = new Guid("53d92c77-3c0e-447c-abc5-0af6cf829a22"),
                            AccountNumber = "AmazonAccountNumber",
                            Denomination = "Amazon",
                            IsValid = true,
                            SortCode = "AAMMZZ"
                        },
                        new
                        {
                            Id = new Guid("11112c77-3c0e-447c-abc5-0af6cf821111"),
                            AccountNumber = "AppleAccountNumber",
                            Denomination = "Apple",
                            IsValid = false,
                            SortCode = "AAPPLL"
                        });
                });

            modelBuilder.Entity("PaymentGateway.Api.Domain.Entities.PaymentStatus", b =>
                {
                    b.Property<Guid>("PaymentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("PaymentId");

                    b.ToTable("PaymentStatuses");
                });
#pragma warning restore 612, 618
        }
    }
}
