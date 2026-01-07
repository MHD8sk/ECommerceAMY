using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAMY.Data.Migrations
{
    public partial class AddDiscountFieldsToCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountAmount",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode", 
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "Carts");
        }
    }
}
