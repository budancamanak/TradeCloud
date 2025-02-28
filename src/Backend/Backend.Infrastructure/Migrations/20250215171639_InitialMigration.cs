using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PluginExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PluginIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TickerId = table.Column<int>(type: "integer", nullable: false),
                    Timeframe = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Progress = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    ParamSet = table.Column<string>(type: "text", nullable: false),
                    TradingParams = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Setting = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTrackLists",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TickerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTrackLists", x => new { x.TickerId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "PluginOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PluginId = table.Column<int>(type: "integer", nullable: false),
                    PluginSignal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SignalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginOutputs_PluginExecutions_PluginId",
                        column: x => x.PluginId,
                        principalTable: "PluginExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginOutputs_PluginId",
                table: "PluginOutputs",
                column: "PluginId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginOutputs");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "UserTrackLists");

            migrationBuilder.DropTable(
                name: "PluginExecutions");
        }
    }
}
