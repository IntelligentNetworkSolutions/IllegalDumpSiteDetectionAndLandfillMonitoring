using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class LandfillWasteEntitiesAddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "legal_landfill_trucks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    registration = table.Column<string>(type: "text", nullable: true),
                    unladen_weight = table.Column<double>(type: "double precision", nullable: true),
                    payload_weight = table.Column<double>(type: "double precision", nullable: true),
                    capacity = table.Column<double>(type: "double precision", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_legal_landfill_trucks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legal_landfill_waste_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_legal_landfill_waste_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legal_landfill_waste_imports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imported_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    import_export_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    capacity = table.Column<double>(type: "double precision", nullable: true),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    legal_landfill_truck_id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_landfill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_landfill_waste_type_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_legal_landfill_waste_imports", x => x.id);
                    table.ForeignKey(
                        name: "fk_legal_landfill_waste_imports_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_legal_landfill_waste_imports_legal_landfill_trucks_legal_la~",
                        column: x => x.legal_landfill_truck_id,
                        principalTable: "legal_landfill_trucks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_legal_landfill_waste_imports_legal_landfill_waste_types_legal_~",
                        column: x => x.legal_landfill_waste_type_id,
                        principalTable: "legal_landfill_waste_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_legal_landfill_waste_imports_legal_landfills_legal_landfill~",
                        column: x => x.legal_landfill_id,
                        principalTable: "legal_landfills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_legal_landfill_waste_imports_created_by_id",
                table: "legal_landfill_waste_imports",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_legal_landfill_waste_imports_legal_landfill_id",
                table: "legal_landfill_waste_imports",
                column: "legal_landfill_id");

            migrationBuilder.CreateIndex(
                name: "IX_legal_landfill_waste_imports_legal_landfill_truck_id",
                table: "legal_landfill_waste_imports",
                column: "legal_landfill_truck_id");

            migrationBuilder.CreateIndex(
                name: "IX_legal_landfill_waste_imports_legal_landfill_waste_type_id",
                table: "legal_landfill_waste_imports",
                column: "legal_landfill_waste_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "legal_landfill_waste_imports");

            migrationBuilder.DropTable(
                name: "legal_landfill_trucks");

            migrationBuilder.DropTable(
                name: "legal_landfill_waste_types");
        }
    }
}
