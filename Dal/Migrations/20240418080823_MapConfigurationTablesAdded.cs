using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class MapConfigurationTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "map_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    map_name = table.Column<string>(type: "text", nullable: false),
                    projection = table.Column<string>(type: "text", nullable: false),
                    tile_grid_js = table.Column<string>(type: "text", nullable: false),
                    center_x = table.Column<double>(type: "double precision", nullable: false),
                    center_y = table.Column<double>(type: "double precision", nullable: false),
                    min_x = table.Column<double>(type: "double precision", nullable: false),
                    min_y = table.Column<double>(type: "double precision", nullable: false),
                    max_x = table.Column<double>(type: "double precision", nullable: false),
                    max_y = table.Column<double>(type: "double precision", nullable: false),
                    resolutions = table.Column<string>(type: "text", nullable: false),
                    default_resolution = table.Column<double>(type: "double precision", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_configurations", x => x.id);
                    table.ForeignKey(
                        name: "fk_map_configurations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_map_configurations_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "map_layer_group_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_name = table.Column<string>(type: "text", nullable: false),
                    layer_group_title_json = table.Column<string>(type: "text", nullable: false),
                    layer_group_description_json = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    opacity = table.Column<double>(type: "double precision", nullable: false),
                    group_js = table.Column<string>(type: "text", nullable: false),
                    map_configuration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MapConfigurationId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_layer_group_configurations", x => x.id);
                    table.ForeignKey(
                        name: "FK_map_layer_group_configurations_map_configurations_MapConfig~",
                        column: x => x.MapConfigurationId1,
                        principalTable: "map_configurations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_map_layer_group_configurations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_map_layer_group_configurations_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_map_layer_group_configurations_map_configurations_map_confi~",
                        column: x => x.map_configuration_id,
                        principalTable: "map_configurations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "map_layer_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    layer_name = table.Column<string>(type: "text", nullable: false),
                    layer_title_json = table.Column<string>(type: "text", nullable: false),
                    layer_description_json = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    layer_js = table.Column<string>(type: "text", nullable: false),
                    map_configuration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    map_layer_group_configuration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MapConfigurationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    MapLayerGroupConfigurationId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_layer_configurations", x => x.id);
                    table.ForeignKey(
                        name: "FK_map_layer_configurations_map_configurations_MapConfiguratio~",
                        column: x => x.MapConfigurationId1,
                        principalTable: "map_configurations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_map_layer_configurations_map_layer_group_configurations_Map~",
                        column: x => x.MapLayerGroupConfigurationId1,
                        principalTable: "map_layer_group_configurations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_map_layer_configurations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_map_layer_configurations_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_map_layer_configurations_map_configurations_map_configurati~",
                        column: x => x.map_configuration_id,
                        principalTable: "map_configurations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_map_layer_configurations_map_layer_group_configurations_map_la~",
                        column: x => x.map_layer_group_configuration_id,
                        principalTable: "map_layer_group_configurations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_configurations_created_by_id",
                table: "map_configurations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_configurations_updated_by_id",
                table: "map_configurations",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_created_by_id",
                table: "map_layer_configurations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_map_configuration_id",
                table: "map_layer_configurations",
                column: "map_configuration_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_map_layer_group_configuration_id",
                table: "map_layer_configurations",
                column: "map_layer_group_configuration_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_MapConfigurationId1",
                table: "map_layer_configurations",
                column: "MapConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_MapLayerGroupConfigurationId1",
                table: "map_layer_configurations",
                column: "MapLayerGroupConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_updated_by_id",
                table: "map_layer_configurations",
                column: "updated_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_group_configurations_created_by_id",
                table: "map_layer_group_configurations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_group_configurations_map_configuration_id",
                table: "map_layer_group_configurations",
                column: "map_configuration_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_group_configurations_MapConfigurationId1",
                table: "map_layer_group_configurations",
                column: "MapConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_group_configurations_updated_by_id",
                table: "map_layer_group_configurations",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_layer_configurations");

            migrationBuilder.DropTable(
                name: "map_layer_group_configurations");

            migrationBuilder.DropTable(
                name: "map_configurations");
        }
    }
}
