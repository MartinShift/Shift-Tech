using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shift_Tech.Migrations
{
    /// <inheritdoc />
    public partial class Checkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_CartId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "Checkouts");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "CartId",
                table: "Checkouts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_CartId",
                table: "Checkouts",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
