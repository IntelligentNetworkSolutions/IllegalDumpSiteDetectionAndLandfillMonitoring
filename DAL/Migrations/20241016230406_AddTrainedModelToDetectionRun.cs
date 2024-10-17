using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainedModelToDetectionRun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "trained_model_id",
                table: "detection_runs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_detection_runs_trained_model_id",
                table: "detection_runs",
                column: "trained_model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_detection_runs_trained_models_trained_model_id",
                table: "detection_runs",
                column: "trained_model_id",
                principalTable: "trained_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_detection_runs_trained_models_trained_model_id",
                table: "detection_runs");

            migrationBuilder.DropIndex(
                name: "IX_detection_runs_trained_model_id",
                table: "detection_runs");

            migrationBuilder.DropColumn(
                name: "trained_model_id",
                table: "detection_runs");
        }
    }
}
