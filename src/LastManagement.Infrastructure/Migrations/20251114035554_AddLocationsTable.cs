using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LastManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    location_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    location_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    location_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.location_id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_locations_active",
                table: "locations",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "idx_locations_code",
                table: "locations",
                column: "location_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_locations_type",
                table: "locations",
                column: "location_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locations");
        }
    }
}
