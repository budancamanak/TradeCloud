using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnalysisExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "PluginIdentifier",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "TickerId",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "Timeframe",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "TradingParams",
                table: "PluginExecutions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PluginExecutions",
                newName: "AnalysisExecutionId");

            migrationBuilder.CreateTable(
                name: "AnalysisExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PluginIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TickerId = table.Column<int>(type: "integer", nullable: false),
                    Timeframe = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Progress = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    ParamSet = table.Column<string>(type: "text", nullable: false),
                    TradingParams = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisExecutions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginExecutions_AnalysisExecutionId",
                table: "PluginExecutions",
                column: "AnalysisExecutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PluginExecutions_AnalysisExecutions_AnalysisExecutionId",
                table: "PluginExecutions",
                column: "AnalysisExecutionId",
                principalTable: "AnalysisExecutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PluginExecutions_AnalysisExecutions_AnalysisExecutionId",
                table: "PluginExecutions");

            migrationBuilder.DropTable(
                name: "AnalysisExecutions");

            migrationBuilder.DropIndex(
                name: "IX_PluginExecutions_AnalysisExecutionId",
                table: "PluginExecutions");

            migrationBuilder.RenameColumn(
                name: "AnalysisExecutionId",
                table: "PluginExecutions",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PluginIdentifier",
                table: "PluginExecutions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TickerId",
                table: "PluginExecutions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Timeframe",
                table: "PluginExecutions",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TradingParams",
                table: "PluginExecutions",
                type: "text",
                nullable: true);
        }
    }
}
