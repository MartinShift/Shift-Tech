using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shift_Tech.Migrations
{
    /// <inheritdoc />
    public partial class CartChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Cart_CartUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_CustomerId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_CustomerId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Products_CartUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CartUserId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CartUserId",
                table: "Purchases",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CartUserId",
                table: "Purchases",
                column: "CartUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Cart_CartUserId",
                table: "Purchases",
                column: "CartUserId",
                principalTable: "Cart",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Cart_CartUserId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_CartUserId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CartUserId",
                table: "Purchases");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Purchases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CartUserId",
                table: "Products",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CustomerId",
                table: "Purchases",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CartUserId",
                table: "Products",
                column: "CartUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Cart_CartUserId",
                table: "Products",
                column: "CartUserId",
                principalTable: "Cart",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_CustomerId",
                table: "Purchases",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
