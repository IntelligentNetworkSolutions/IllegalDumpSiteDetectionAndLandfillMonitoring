using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredDumpsiteStatusRiskWasteTypeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_resolvment",
                table: "registered_dumpsites",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "registered_dumpsite_risk_level_id",
                table: "registered_dumpsites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "registered_dumpsite_status_id",
                table: "registered_dumpsites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "registered_dumpsite_waste_type_id",
                table: "registered_dumpsites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_risk_levels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_risk_levels", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_risk_levels_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_statuses",
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
                    table.PrimaryKey("pk_registered_dumpsite_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "registered_dumpsite_waste_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_registered_dumpsite_waste_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_registered_dumpsite_waste_types_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "registered_dumpsite_statuses",
                columns: new[] { "id", "color", "description", "name" },
                values: new object[,]
                {
                    { 1, "#FF0000", "Waste has been detected.", "Detected" },
                    { 2, "#FFA500", "Cleanup is in progress.", "InProcess" },
                    { 3, "#008000", "Waste has been resolved.", "Resolved" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_risk_level_id",
                table: "registered_dumpsites",
                column: "registered_dumpsite_risk_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_status_id",
                table: "registered_dumpsites",
                column: "registered_dumpsite_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_waste_type_id",
                table: "registered_dumpsites",
                column: "registered_dumpsite_waste_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_risk_levels_created_by_id",
                table: "registered_dumpsite_risk_levels",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_registered_dumpsite_waste_types_created_by_id",
                table: "registered_dumpsite_waste_types",
                column: "created_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_risk_levels_registere~",
                table: "registered_dumpsites",
                column: "registered_dumpsite_risk_level_id",
                principalTable: "registered_dumpsite_risk_levels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_statuses_registered_~",
                table: "registered_dumpsites",
                column: "registered_dumpsite_status_id",
                principalTable: "registered_dumpsite_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_waste_types_registere~",
                table: "registered_dumpsites",
                column: "registered_dumpsite_waste_type_id",
                principalTable: "registered_dumpsite_waste_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_risk_levels_registere~",
                table: "registered_dumpsites");

            migrationBuilder.DropForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_statuses_registered_~",
                table: "registered_dumpsites");

            migrationBuilder.DropForeignKey(
                name: "fk_registered_dumpsites_registered_dumpsite_waste_types_registere~",
                table: "registered_dumpsites");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_risk_levels");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_statuses");

            migrationBuilder.DropTable(
                name: "registered_dumpsite_waste_types");

            migrationBuilder.DropIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_risk_level_id",
                table: "registered_dumpsites");

            migrationBuilder.DropIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_status_id",
                table: "registered_dumpsites");

            migrationBuilder.DropIndex(
                name: "IX_registered_dumpsites_registered_dumpsite_waste_type_id",
                table: "registered_dumpsites");

            migrationBuilder.DropColumn(
                name: "date_of_resolvment",
                table: "registered_dumpsites");

            migrationBuilder.DropColumn(
                name: "registered_dumpsite_risk_level_id",
                table: "registered_dumpsites");

            migrationBuilder.DropColumn(
                name: "registered_dumpsite_status_id",
                table: "registered_dumpsites");

            migrationBuilder.DropColumn(
                name: "registered_dumpsite_waste_type_id",
                table: "registered_dumpsites");
        }
    }
}
