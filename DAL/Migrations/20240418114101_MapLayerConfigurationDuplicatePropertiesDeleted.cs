using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class MapLayerConfigurationDuplicatePropertiesDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_map_layer_configurations_map_configurations_MapConfiguratio~",
                table: "map_layer_configurations");

            migrationBuilder.DropForeignKey(
                name: "FK_map_layer_configurations_map_layer_group_configurations_Map~",
                table: "map_layer_configurations");

            migrationBuilder.DropIndex(
                name: "IX_map_layer_configurations_MapConfigurationId1",
                table: "map_layer_configurations");

            migrationBuilder.DropIndex(
                name: "IX_map_layer_configurations_MapLayerGroupConfigurationId1",
                table: "map_layer_configurations");

            migrationBuilder.DropColumn(
                name: "MapConfigurationId1",
                table: "map_layer_configurations");

            migrationBuilder.DropColumn(
                name: "MapLayerGroupConfigurationId1",
                table: "map_layer_configurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MapConfigurationId1",
                table: "map_layer_configurations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MapLayerGroupConfigurationId1",
                table: "map_layer_configurations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_MapConfigurationId1",
                table: "map_layer_configurations",
                column: "MapConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_configurations_MapLayerGroupConfigurationId1",
                table: "map_layer_configurations",
                column: "MapLayerGroupConfigurationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_map_layer_configurations_map_configurations_MapConfiguratio~",
                table: "map_layer_configurations",
                column: "MapConfigurationId1",
                principalTable: "map_configurations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_map_layer_configurations_map_layer_group_configurations_Map~",
                table: "map_layer_configurations",
                column: "MapLayerGroupConfigurationId1",
                principalTable: "map_layer_group_configurations",
                principalColumn: "id");
        }
    }
}
