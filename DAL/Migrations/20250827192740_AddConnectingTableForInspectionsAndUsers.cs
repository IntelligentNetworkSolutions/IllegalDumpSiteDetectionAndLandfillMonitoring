using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectingTableForInspectionsAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_registered_dumpsite_inspections_asp_net_users_inspector_id",
                table: "registered_dumpsite_inspections");

            migrationBuilder.DropIndex(
                name: "IX_registered_dumpsite_inspections_inspector_id",
                table: "registered_dumpsite_inspections");

            migrationBuilder.DropColumn(
                name: "inspector_id",
                table: "registered_dumpsite_inspections");

            migrationBuilder.CreateTable(
                name: "inspection_assignment",
                columns: table => new
                {
                    registered_dumpsite_inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspector_id = table.Column<string>(type: "text", nullable: false),
                    assigned_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_assignment", x => new { x.registered_dumpsite_inspection_id, x.inspector_id });
                    table.ForeignKey(
                        name: "fk_inspection_assignment_asp_net_users_inspector_id",
                        column: x => x.inspector_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inspection_assignment_registered_dumpsite_inspections_registe~",
                        column: x => x.registered_dumpsite_inspection_id,
                        principalTable: "registered_dumpsite_inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inspection_assignment_inspector_id",
                table: "inspection_assignment",
                column: "inspector_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inspection_assignment");

            migrationBuilder.AddColumn<string>(
                name: "inspector_id",
                table: "registered_dumpsite_inspections",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspections_inspector_id",
                table: "registered_dumpsite_inspections",
                column: "inspector_id");

            migrationBuilder.AddForeignKey(
                name: "fk_registered_dumpsite_inspections_asp_net_users_inspector_id",
                table: "registered_dumpsite_inspections",
                column: "inspector_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
