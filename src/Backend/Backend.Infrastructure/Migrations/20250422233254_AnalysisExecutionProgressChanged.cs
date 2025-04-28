using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnalysisExecutionProgressChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "AnalysisExecutions");

            migrationBuilder.AddColumn<int>(
                name: "ProgressCurrent",
                table: "AnalysisExecutions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgressTotal",
                table: "AnalysisExecutions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressCurrent",
                table: "AnalysisExecutions");

            migrationBuilder.DropColumn(
                name: "ProgressTotal",
                table: "AnalysisExecutions");

            migrationBuilder.AddColumn<double>(
                name: "Progress",
                table: "AnalysisExecutions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
