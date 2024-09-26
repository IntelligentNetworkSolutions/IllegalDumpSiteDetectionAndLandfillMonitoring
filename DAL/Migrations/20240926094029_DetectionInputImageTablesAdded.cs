using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DetectionInputImageTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_file_name",
                table: "detection_runs");

            migrationBuilder.DropColumn(
                name: "image_path",
                table: "detection_runs");

            migrationBuilder.AddColumn<Guid>(
                name: "detection_input_image_id",
                table: "detection_runs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "detection_input_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    date_taken = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    image_file_name = table.Column<string>(type: "text", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detection_input_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_detection_input_images_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_detection_input_images_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detection_runs_detection_input_image_id",
                table: "detection_runs",
                column: "detection_input_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_detection_input_images_created_by_id",
                table: "detection_input_images",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_detection_input_images_updated_by_id",
                table: "detection_input_images",
                column: "updated_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_detection_runs_detection_input_images_detection_input_image~",
                table: "detection_runs",
                column: "detection_input_image_id",
                principalTable: "detection_input_images",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_detection_runs_detection_input_images_detection_input_image~",
                table: "detection_runs");

            migrationBuilder.DropTable(
                name: "detection_input_images");

            migrationBuilder.DropIndex(
                name: "IX_detection_runs_detection_input_image_id",
                table: "detection_runs");

            migrationBuilder.DropColumn(
                name: "detection_input_image_id",
                table: "detection_runs");

            migrationBuilder.AddColumn<string>(
                name: "image_file_name",
                table: "detection_runs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "image_path",
                table: "detection_runs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
