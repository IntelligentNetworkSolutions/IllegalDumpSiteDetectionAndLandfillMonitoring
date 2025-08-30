using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredDumpsiteFileAndInspectionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registered_dumpsite_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_dumpsite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    original_file_name = table.Column<string>(type: "text", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    file_extension = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_files_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_files_registered_dumpsites_registered_d~",
                        column: x => x.registered_dumpsite_id,
                        principalTable: "registered_dumpsites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_inspection_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_inspection_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_inspections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_dumpsite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspector_id = table.Column<string>(type: "text", nullable: false),
                    inspection_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    registered_dumpsite_inspection_status_id = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    findings = table.Column<string>(type: "text", nullable: true),
                    recommendations = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_inspections", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspections_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspections_asp_net_users_inspector_id",
                        column: x => x.inspector_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspections_registered_dumpsite_inspectio~",
                        column: x => x.registered_dumpsite_inspection_status_id,
                        principalTable: "registered_dumpsite_inspection_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspections_registered_dumpsites_regist~",
                        column: x => x.registered_dumpsite_id,
                        principalTable: "registered_dumpsites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_inspection_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_dumpsite_inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    original_file_name = table.Column<string>(type: "text", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    file_extension = table.Column<string>(type: "text", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_inspection_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspection_files_asp_net_users_created_~",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_inspection_files_registered_dumpsite_in~",
                        column: x => x.registered_dumpsite_inspection_id,
                        principalTable: "registered_dumpsite_inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "registered_dumpsite_inspection_statuses",
                columns: new[] { "id", "color", "description", "name" },
                values: new object[,]
                {
                    { 1, "#1E90FF", "Cleanup has been scheduled.", "Scheduled" },
                    { 2, "#800080", "Cleanup task has been assigned.", "Assigned" },
                    { 3, "#FFD700", "Cleanup is currently in progress.", "InProgress" },
                    { 4, "#228B22", "Cleanup has been completed.", "Completed" },
                    { 5, "#A9A9A9", "Cleanup has been cancelled.", "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_files_created_by_id",
                table: "registered_dumpsite_files",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_files_registered_dumpsite_id",
                table: "registered_dumpsite_files",
                column: "registered_dumpsite_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspection_files_created_by_id",
                table: "registered_dumpsite_inspection_files",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspection_files_registered_dumpsite_in~",
                table: "registered_dumpsite_inspection_files",
                column: "registered_dumpsite_inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspections_created_by_id",
                table: "registered_dumpsite_inspections",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspections_inspector_id",
                table: "registered_dumpsite_inspections",
                column: "inspector_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspections_registered_dumpsite_id",
                table: "registered_dumpsite_inspections",
                column: "registered_dumpsite_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_inspections_registered_dumpsite_inspect~",
                table: "registered_dumpsite_inspections",
                column: "registered_dumpsite_inspection_status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registered_dumpsite_files");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_inspection_files");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_inspections");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_inspection_statuses");
        }
    }
}
