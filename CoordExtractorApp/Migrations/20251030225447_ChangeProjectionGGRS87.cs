using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProjectionGGRS87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Geometry>(
                name: "Geom",
                table: "ConversionJobs",
                type: "geometry(Polygon, 2100)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Polygon, 4326)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Geometry>(
                name: "Geom",
                table: "ConversionJobs",
                type: "geometry(Polygon, 4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Polygon, 2100)");
        }
    }
}
