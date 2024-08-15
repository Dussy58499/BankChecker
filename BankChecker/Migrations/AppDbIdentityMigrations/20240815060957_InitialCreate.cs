using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankChecker.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FullExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true),
                    CurrencyName = table.Column<string>(nullable: true),
                    CurrencyUahCode = table.Column<string>(nullable: true),
                    CurrencyUahName = table.Column<string>(nullable: true),
                    BuyRate = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    SellRate = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    BankName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FullExchangeRates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FullExchangeRates");
        }
    }
}
