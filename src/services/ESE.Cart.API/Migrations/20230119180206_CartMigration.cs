using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESE.Cart.API.Migrations
{
    public partial class CartMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CartId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Image = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItens_CartClients_CartId",
                        column: x => x.CartId,
                        principalTable: "CartClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Client",
                table: "CartClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItens_CartId",
                table: "CartItens",
                column: "CartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItens");

            migrationBuilder.DropTable(
                name: "CartClients");
        }
    }
}
