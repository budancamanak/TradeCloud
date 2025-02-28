using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ConnectionUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    DecimalPoint = table.Column<int>(type: "integer", nullable: false),
                    ExchangeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickers_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TickerId = table.Column<int>(type: "integer", nullable: false),
                    Timeframe = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    High = table.Column<double>(type: "double precision", nullable: false),
                    Low = table.Column<double>(type: "double precision", nullable: false),
                    Open = table.Column<double>(type: "double precision", nullable: false),
                    Close = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.Timestamp, x.TickerId, x.Timeframe });
                    table.ForeignKey(
                        name: "FK_Prices_Tickers_TickerId",
                        column: x => x.TickerId,
                        principalTable: "Tickers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_TickerId",
                table: "Prices",
                column: "TickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickers_ExchangeId",
                table: "Tickers",
                column: "ExchangeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Tickers");

            migrationBuilder.DropTable(
                name: "Exchanges");
        }
    }
}
