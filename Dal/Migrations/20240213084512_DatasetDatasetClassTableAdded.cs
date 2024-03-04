using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DatasetDatasetClassTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes");

            migrationBuilder.DropIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes");

            migrationBuilder.DropColumn(
                name: "dataset_id",
                table: "dataset_classes");

            migrationBuilder.CreateTable(
                name: "datasets_dataset_classes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dataset_class_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_datasets_dataset_classes", x => x.id);
                    table.ForeignKey(
                        name: "fk_datasets_dataset_classes_dataset_classes_dataset_class_id",
                        column: x => x.dataset_class_id,
                        principalTable: "dataset_classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_datasets_dataset_classes_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_datasets_dataset_classes_dataset_class_id",
                table: "datasets_dataset_classes",
                column: "dataset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_datasets_dataset_classes_dataset_id",
                table: "datasets_dataset_classes",
                column: "dataset_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "datasets_dataset_classes");

            migrationBuilder.AddColumn<Guid>(
                name: "dataset_id",
                table: "dataset_classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes",
                column: "dataset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id");
        }
    }
}
