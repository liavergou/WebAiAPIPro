using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectConversionJobRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CroppedFileName",
                table: "ConversionJobs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ConversionJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ConversionJobs_ProjectId",
                table: "ConversionJobs",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversionJobs_Projects_ProjectId",
                table: "ConversionJobs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversionJobs_Projects_ProjectId",
                table: "ConversionJobs");

            migrationBuilder.DropIndex(
                name: "IX_ConversionJobs_ProjectId",
                table: "ConversionJobs");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ConversionJobs");

            migrationBuilder.AlterColumn<string>(
                name: "CroppedFileName",
                table: "ConversionJobs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
