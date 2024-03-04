using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationOneToManyDatasetImagesAndApplicationUserPlusNewPropertyNameAddedInDatasetImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dataset_images_created_by_id",
                table: "dataset_images");

            migrationBuilder.DropIndex(
                name: "IX_dataset_images_updated_by_id",
                table: "dataset_images");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "dataset_images",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_created_by_id",
                table: "dataset_images",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_updated_by_id",
                table: "dataset_images",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dataset_images_created_by_id",
                table: "dataset_images");

            migrationBuilder.DropIndex(
                name: "IX_dataset_images_updated_by_id",
                table: "dataset_images");

            migrationBuilder.DropColumn(
                name: "name",
                table: "dataset_images");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_created_by_id",
                table: "dataset_images",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_updated_by_id",
                table: "dataset_images",
                column: "updated_by_id",
                unique: true);
        }
    }
}
