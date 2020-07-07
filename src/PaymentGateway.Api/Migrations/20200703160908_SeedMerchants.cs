using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Api.Migrations
{
    public partial class SeedMerchants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "Id", "AccountNumber", "Denomination", "IsValid", "SortCode" },
                values: new object[] { new Guid("53d92c77-3c0e-447c-abc5-0af6cf829a22"), "AmazonAccountNumber", "Amazon", true, "AAMMZZ" });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "Id", "AccountNumber", "Denomination", "IsValid", "SortCode" },
                values: new object[] { new Guid("11112c77-3c0e-447c-abc5-0af6cf821111"), "AppleAccountNumber", "Apple", false, "AAPPLL" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: new Guid("11112c77-3c0e-447c-abc5-0af6cf821111"));

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: new Guid("53d92c77-3c0e-447c-abc5-0af6cf829a22"));
        }
    }
}
