using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationOneToManyDatasetImagesAndDatasets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dataset_images_dataset_id",
                table: "dataset_images");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_dataset_id",
                table: "dataset_images",
                column: "dataset_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dataset_images_dataset_id",
                table: "dataset_images");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_dataset_id",
                table: "dataset_images",
                column: "dataset_id",
                unique: true);
        }
    }
}
