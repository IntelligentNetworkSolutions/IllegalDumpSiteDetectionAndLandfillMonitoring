using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WeakenDatasetTrainingRelationDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trained_models_datasets_dataset_id",
                table: "trained_models");

            migrationBuilder.DropForeignKey(
                name: "fk_training_runs_datasets_dataset_id",
                table: "training_runs");

            migrationBuilder.AddForeignKey(
                name: "fk_trained_models_datasets_dataset_id",
                table: "trained_models",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_training_runs_datasets_dataset_id",
                table: "training_runs",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trained_models_datasets_dataset_id",
                table: "trained_models");

            migrationBuilder.DropForeignKey(
                name: "fk_training_runs_datasets_dataset_id",
                table: "training_runs");

            migrationBuilder.AddForeignKey(
                name: "fk_trained_models_datasets_dataset_id",
                table: "trained_models",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_training_runs_datasets_dataset_id",
                table: "training_runs",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
