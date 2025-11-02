using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAuditByUserBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ConversionJobs");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "ConversionJobs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ConversionJobs");

            migrationBuilder.RenameColumn(
                name: "ImageFileId",
                table: "ConversionJobs",
                newName: "MongoImageFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MongoImageFileId",
                table: "ConversionJobs",
                newName: "ImageFileId");

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Prompts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Prompts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Prompts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Projects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ConversionJobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "ConversionJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ConversionJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
