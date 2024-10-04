using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasetClassToImageAnnotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_image_annotations_dataset_image_id",
                table: "image_annotations");

            migrationBuilder.DropColumn(
                name: "annotations_geo_json",
                table: "image_annotations");

            migrationBuilder.AlterColumn<Polygon>(
                name: "geom",
                table: "image_annotations",
                type: "geometry(Polygon)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.AddColumn<Guid>(
                name: "dataset_class_id",
                table: "image_annotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_dataset_class_id",
                table: "image_annotations",
                column: "dataset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_dataset_image_id",
                table: "image_annotations",
                column: "dataset_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_image_annotations_dataset_classes_dataset_class_id",
                table: "image_annotations",
                column: "dataset_class_id",
                principalTable: "dataset_classes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_image_annotations_dataset_classes_dataset_class_id",
                table: "image_annotations");

            migrationBuilder.DropIndex(
                name: "IX_image_annotations_dataset_class_id",
                table: "image_annotations");

            migrationBuilder.DropIndex(
                name: "IX_image_annotations_dataset_image_id",
                table: "image_annotations");

            migrationBuilder.DropColumn(
                name: "dataset_class_id",
                table: "image_annotations");

            migrationBuilder.AlterColumn<Geometry>(
                name: "geom",
                table: "image_annotations",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Polygon),
                oldType: "geometry(Polygon)");

            migrationBuilder.AddColumn<string>(
                name: "annotations_geo_json",
                table: "image_annotations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_dataset_image_id",
                table: "image_annotations",
                column: "dataset_image_id",
                unique: true);
        }
    }
}
