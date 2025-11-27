using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorJobConversionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MongoImageFileId",
                table: "ConversionJobs",
                newName: "CroppedFileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CroppedFileName",
                table: "ConversionJobs",
                newName: "MongoImageFileId");
        }
    }
}
