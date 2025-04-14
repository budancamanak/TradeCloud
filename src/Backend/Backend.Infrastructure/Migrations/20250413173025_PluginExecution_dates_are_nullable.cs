using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PluginExecution_dates_are_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueuedDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RunStartDate",
                table: "PluginExecutions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "QueuedDate",
                table: "PluginExecutions");

            migrationBuilder.DropColumn(
                name: "RunStartDate",
                table: "PluginExecutions");
        }
    }
}
