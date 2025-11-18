using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LastManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInventoryStockIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "inventory_stocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "inventory_stocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
