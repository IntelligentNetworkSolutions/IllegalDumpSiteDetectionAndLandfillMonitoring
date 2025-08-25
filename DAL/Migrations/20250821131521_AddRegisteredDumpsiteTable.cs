using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using System;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredDumpsiteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registered_dumpsites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    geom = table.Column<Polygon>(type: "geometry(Polygon)", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsites", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsites_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsites_created_by_id",
                table: "registered_dumpsites",
                column: "created_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registered_dumpsites");
        }
    }
}
