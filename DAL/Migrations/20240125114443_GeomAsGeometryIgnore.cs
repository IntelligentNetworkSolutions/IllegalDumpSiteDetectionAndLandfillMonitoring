using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class GeomAsGeometryIgnore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Geometry>(
                name: "geom",
                table: "image_annotations",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "geom",
                table: "image_annotations",
                type: "text",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");
        }
    }
}
