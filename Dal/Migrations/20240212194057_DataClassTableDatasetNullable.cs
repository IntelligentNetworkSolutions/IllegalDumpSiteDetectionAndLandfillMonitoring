using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DataClassTableDatasetNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes");

            migrationBuilder.AlterColumn<Guid>(
                name: "dataset_id",
                table: "dataset_classes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes");

            migrationBuilder.AlterColumn<Guid>(
                name: "dataset_id",
                table: "dataset_classes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_dataset_classes_datasets_dataset_id",
                table: "dataset_classes",
                column: "dataset_id",
                principalTable: "datasets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
