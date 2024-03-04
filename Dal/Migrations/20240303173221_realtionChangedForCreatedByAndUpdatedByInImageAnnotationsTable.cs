using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class realtionChangedForCreatedByAndUpdatedByInImageAnnotationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_image_annotations_created_by_id",
                table: "image_annotations");

            migrationBuilder.DropIndex(
                name: "IX_image_annotations_updated_by_id",
                table: "image_annotations");

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_created_by_id",
                table: "image_annotations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_updated_by_id",
                table: "image_annotations",
                column: "updated_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_image_annotations_created_by_id",
                table: "image_annotations");

            migrationBuilder.DropIndex(
                name: "IX_image_annotations_updated_by_id",
                table: "image_annotations");

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_created_by_id",
                table: "image_annotations",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_updated_by_id",
                table: "image_annotations",
                column: "updated_by_id",
                unique: true);
        }
    }
}
