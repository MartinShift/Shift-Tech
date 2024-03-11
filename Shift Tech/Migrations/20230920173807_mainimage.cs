using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shift_Tech.Migrations
{
    /// <inheritdoc />
    public partial class mainimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "Products",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_MainImageId",
                table: "Products",
                column: "MainImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Images_MainImageId",
                table: "Products",
                column: "MainImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Images_MainImageId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MainImageId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Products");
        }
    }
}
