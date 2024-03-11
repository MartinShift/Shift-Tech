using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shift_Tech.Migrations
{
    /// <inheritdoc />
    public partial class checkoutId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Checkouts_CheckoutId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CheckoutId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CheckoutId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CheckoutId",
                table: "CartProducts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartProducts_CheckoutId",
                table: "CartProducts",
                column: "CheckoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartProducts_Checkouts_CheckoutId",
                table: "CartProducts",
                column: "CheckoutId",
                principalTable: "Checkouts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProducts_Checkouts_CheckoutId",
                table: "CartProducts");

            migrationBuilder.DropIndex(
                name: "IX_CartProducts_CheckoutId",
                table: "CartProducts");

            migrationBuilder.DropColumn(
                name: "CheckoutId",
                table: "CartProducts");

            migrationBuilder.AddColumn<int>(
                name: "CheckoutId",
                table: "Products",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CheckoutId",
                table: "Products",
                column: "CheckoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Checkouts_CheckoutId",
                table: "Products",
                column: "CheckoutId",
                principalTable: "Checkouts",
                principalColumn: "Id");
        }
    }
}
