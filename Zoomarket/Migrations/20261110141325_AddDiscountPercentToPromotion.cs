using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zoomarket.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountPercentToPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Promotions");
        }
    }
}
