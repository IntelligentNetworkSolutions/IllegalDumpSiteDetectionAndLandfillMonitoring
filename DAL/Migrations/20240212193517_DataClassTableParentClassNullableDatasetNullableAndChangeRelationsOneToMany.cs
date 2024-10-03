using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DataClassTableParentClassNullableDatasetNullableAndChangeRelationsOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_created_by_id",
                table: "dataset_classes");

            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes");

            migrationBuilder.DropColumn(
                name: "class_id",
                table: "dataset_classes");

            migrationBuilder.AddColumn<Guid>(
                name: "parent_class_id",
                table: "dataset_classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_created_by_id",
                table: "dataset_classes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes",
                column: "dataset_id");

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_parent_class_id",
                table: "dataset_classes",
                column: "parent_class_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dataset_classes_dataset_classes_parent_class_id",
                table: "dataset_classes",
                column: "parent_class_id",
                principalTable: "dataset_classes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dataset_classes_dataset_classes_parent_class_id",
                table: "dataset_classes");

            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_created_by_id",
                table: "dataset_classes");

            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes");

            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_parent_class_id",
                table: "dataset_classes");

            migrationBuilder.DropColumn(
                name: "parent_class_id",
                table: "dataset_classes");

            migrationBuilder.AddColumn<int>(
                name: "class_id",
                table: "dataset_classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_created_by_id",
                table: "dataset_classes",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes",
                column: "dataset_id",
                unique: true);
        }
    }
}
