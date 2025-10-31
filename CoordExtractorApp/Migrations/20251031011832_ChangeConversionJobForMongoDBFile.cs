using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeConversionJobForMongoDBFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CroppedFileName",
                table: "ConversionJobs");

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "ConversionJobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ImageFileId",
                table: "ConversionJobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geom",
                table: "ConversionJobs",
                type: "geometry(Polygon, 2100)",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Polygon, 2100)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ConversionJobs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ConversionJobs");

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "ConversionJobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageFileId",
                table: "ConversionJobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geom",
                table: "ConversionJobs",
                type: "geometry(Polygon, 2100)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Polygon, 2100)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CroppedFileName",
                table: "ConversionJobs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
