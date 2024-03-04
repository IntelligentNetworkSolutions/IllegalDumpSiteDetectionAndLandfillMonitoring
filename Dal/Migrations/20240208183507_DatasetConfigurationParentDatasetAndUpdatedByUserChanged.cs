using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DatasetConfigurationParentDatasetAndUpdatedByUserChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_datasets_created_by_id",
                table: "datasets");

            migrationBuilder.DropIndex(
                name: "IX_datasets_parent_dataset_id",
                table: "datasets");

            migrationBuilder.DropIndex(
                name: "IX_datasets_updated_by_id",
                table: "datasets");

            migrationBuilder.CreateIndex(
                name: "IX_datasets_created_by_id",
                table: "datasets",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_datasets_parent_dataset_id",
                table: "datasets",
                column: "parent_dataset_id");

            migrationBuilder.CreateIndex(
                name: "IX_datasets_updated_by_id",
                table: "datasets",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_datasets_created_by_id",
                table: "datasets");

            migrationBuilder.DropIndex(
                name: "IX_datasets_parent_dataset_id",
                table: "datasets");

            migrationBuilder.DropIndex(
                name: "IX_datasets_updated_by_id",
                table: "datasets");

            migrationBuilder.CreateIndex(
                name: "IX_datasets_created_by_id",
                table: "datasets",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datasets_parent_dataset_id",
                table: "datasets",
                column: "parent_dataset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datasets_updated_by_id",
                table: "datasets",
                column: "updated_by_id",
                unique: true);
        }
    }
}
