using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LastManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSizesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "last_sizes",
                columns: table => new
                {
                    size_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    size_value = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: false),
                    size_label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    replacement_size_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_last_sizes", x => x.size_id);
                    table.ForeignKey(
                        name: "FK_last_sizes_last_sizes_replacement_size_id",
                        column: x => x.replacement_size_id,
                        principalTable: "last_sizes",
                        principalColumn: "size_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_last_sizes_status",
                table: "last_sizes",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_last_sizes_value",
                table: "last_sizes",
                column: "size_value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_last_sizes_replacement_size_id",
                table: "last_sizes",
                column: "replacement_size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "last_sizes");
        }
    }
}
