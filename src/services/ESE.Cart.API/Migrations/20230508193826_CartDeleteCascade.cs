using Microsoft.EntityFrameworkCore.Migrations;

namespace ESE.Cart.API.Migrations
{
    public partial class CartDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_CartClients_CartId",
                table: "CartItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_CartClients_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "CartClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_CartClients_CartId",
                table: "CartItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_CartClients_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "CartClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
