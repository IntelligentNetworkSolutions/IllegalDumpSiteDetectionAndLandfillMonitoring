using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class MapLayerGroupConfigurationDuplicatePropertiesDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_map_layer_group_configurations_map_configurations_MapConfig~",
                table: "map_layer_group_configurations");

            migrationBuilder.DropIndex(
                name: "IX_map_layer_group_configurations_MapConfigurationId1",
                table: "map_layer_group_configurations");

            migrationBuilder.DropColumn(
                name: "MapConfigurationId1",
                table: "map_layer_group_configurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MapConfigurationId1",
                table: "map_layer_group_configurations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_map_layer_group_configurations_MapConfigurationId1",
                table: "map_layer_group_configurations",
                column: "MapConfigurationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_map_layer_group_configurations_map_configurations_MapConfig~",
                table: "map_layer_group_configurations",
                column: "MapConfigurationId1",
                principalTable: "map_configurations",
                principalColumn: "id");
        }
    }
}
