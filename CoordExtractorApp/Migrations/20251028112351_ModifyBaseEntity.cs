using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConversionJobs");

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Prompts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Prompts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Prompts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ConversionJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsertedBy",
                table: "ConversionJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ConversionJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Prompts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConversionJobs",
                type: "bit",
                nullable: true);
        }
    }
}
