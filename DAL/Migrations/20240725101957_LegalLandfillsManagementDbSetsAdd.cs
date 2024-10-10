using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class LegalLandfillsManagementDbSetsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalLandfillPointCloudFile_LegalLandfill_LegalLandfillId",
                table: "LegalLandfillPointCloudFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LegalLandfillPointCloudFile",
                table: "LegalLandfillPointCloudFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LegalLandfill",
                table: "LegalLandfill");

            migrationBuilder.RenameTable(
                name: "LegalLandfillPointCloudFile",
                newName: "legal_landfill_point_cloud_files");

            migrationBuilder.RenameTable(
                name: "LegalLandfill",
                newName: "legal_landfills");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "legal_landfill_point_cloud_files",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ScanDateTime",
                table: "legal_landfill_point_cloud_files",
                newName: "scan_date_time");

            migrationBuilder.RenameColumn(
                name: "LegalLandfillId",
                table: "legal_landfill_point_cloud_files",
                newName: "legal_landfill_id");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "legal_landfill_point_cloud_files",
                newName: "file_name");

            migrationBuilder.RenameIndex(
                name: "IX_LegalLandfillPointCloudFile_LegalLandfillId",
                table: "legal_landfill_point_cloud_files",
                newName: "IX_legal_landfill_point_cloud_files_legal_landfill_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "legal_landfills",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "legal_landfills",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "legal_landfills",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_legal_landfill_point_cloud_files",
                table: "legal_landfill_point_cloud_files",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_legal_landfills",
                table: "legal_landfills",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_legal_landfill_point_cloud_files_legal_landfills_legal_land~",
                table: "legal_landfill_point_cloud_files",
                column: "legal_landfill_id",
                principalTable: "legal_landfills",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_legal_landfill_point_cloud_files_legal_landfills_legal_land~",
                table: "legal_landfill_point_cloud_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_legal_landfills",
                table: "legal_landfills");

            migrationBuilder.DropPrimaryKey(
                name: "pk_legal_landfill_point_cloud_files",
                table: "legal_landfill_point_cloud_files");

            migrationBuilder.RenameTable(
                name: "legal_landfills",
                newName: "LegalLandfill");

            migrationBuilder.RenameTable(
                name: "legal_landfill_point_cloud_files",
                newName: "LegalLandfillPointCloudFile");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "LegalLandfill",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "LegalLandfill",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "LegalLandfill",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "LegalLandfillPointCloudFile",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "scan_date_time",
                table: "LegalLandfillPointCloudFile",
                newName: "ScanDateTime");

            migrationBuilder.RenameColumn(
                name: "legal_landfill_id",
                table: "LegalLandfillPointCloudFile",
                newName: "LegalLandfillId");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "LegalLandfillPointCloudFile",
                newName: "FileName");

            migrationBuilder.RenameIndex(
                name: "IX_legal_landfill_point_cloud_files_legal_landfill_id",
                table: "LegalLandfillPointCloudFile",
                newName: "IX_LegalLandfillPointCloudFile_LegalLandfillId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LegalLandfill",
                table: "LegalLandfill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LegalLandfillPointCloudFile",
                table: "LegalLandfillPointCloudFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalLandfillPointCloudFile_LegalLandfill_LegalLandfillId",
                table: "LegalLandfillPointCloudFile",
                column: "LegalLandfillId",
                principalTable: "LegalLandfill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
