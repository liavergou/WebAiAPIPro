using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorMessageFieldToConversionJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "ConversionJobs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "ConversionJobs");
        }
    }
}
