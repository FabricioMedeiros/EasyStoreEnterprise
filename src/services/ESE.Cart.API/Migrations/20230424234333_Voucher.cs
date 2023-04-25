using Microsoft.EntityFrameworkCore.Migrations;

namespace ESE.Cart.API.Migrations
{
    public partial class Voucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "CartClients",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "VoucherUsed",
                table: "CartClients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "CartClients",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "CartClients",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Percent",
                table: "CartClients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeDiscount",
                table: "CartClients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "CartClients");

            migrationBuilder.DropColumn(
                name: "VoucherUsed",
                table: "CartClients");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "CartClients");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "CartClients");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "CartClients");

            migrationBuilder.DropColumn(
                name: "TypeDiscount",
                table: "CartClients");
        }
    }
}
