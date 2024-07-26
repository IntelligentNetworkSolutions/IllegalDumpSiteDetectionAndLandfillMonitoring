using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class LegalLandfillsManagementTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegalLandfill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalLandfill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LegalLandfillPointCloudFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ScanDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    LegalLandfillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalLandfillPointCloudFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalLandfillPointCloudFile_LegalLandfill_LegalLandfillId",
                        column: x => x.LegalLandfillId,
                        principalTable: "LegalLandfill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalLandfillPointCloudFile_LegalLandfillId",
                table: "LegalLandfillPointCloudFile",
                column: "LegalLandfillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegalLandfillPointCloudFile");

            migrationBuilder.DropTable(
                name: "LegalLandfill");
        }
    }
}
