using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LastManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryStocksAndMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventory_movements",
                columns: table => new
                {
                    movement_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_id = table.Column<int>(type: "integer", nullable: false),
                    size_id = table.Column<int>(type: "integer", nullable: false),
                    from_location_id = table.Column<int>(type: "integer", nullable: true),
                    to_location_id = table.Column<int>(type: "integer", nullable: true),
                    movement_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    reference_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_movements", x => x.movement_id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_stocks",
                columns: table => new
                {
                    stock_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_id = table.Column<int>(type: "integer", nullable: false),
                    size_id = table.Column<int>(type: "integer", nullable: false),
                    location_id = table.Column<int>(type: "integer", nullable: false),
                    quantity_good = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    quantity_damaged = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    quantity_reserved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_stocks", x => x.stock_id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_movements_last",
                table: "inventory_movements",
                columns: new[] { "last_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_movements_reference",
                table: "inventory_movements",
                column: "reference_number",
                filter: "reference_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_movements_type",
                table: "inventory_movements",
                columns: new[] { "movement_type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_inventory_last_size",
                table: "inventory_stocks",
                columns: new[] { "last_id", "size_id" });

            migrationBuilder.CreateIndex(
                name: "idx_inventory_location",
                table: "inventory_stocks",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "uq_inventory_stocks_composite",
                table: "inventory_stocks",
                columns: new[] { "last_id", "size_id", "location_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_movements");

            migrationBuilder.DropTable(
                name: "inventory_stocks");
        }
    }
}
