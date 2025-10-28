using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoordExtractorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddConvertionJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversionJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CroppedFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelUsed = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WktOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFileId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PromptId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    InsertedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversionJobs_Prompts_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConversionJobs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionJobs_PromptId",
                table: "ConversionJobs",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionJobs_UserId",
                table: "ConversionJobs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversionJobs");
        }
    }
}
