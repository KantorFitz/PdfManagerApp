using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfManagerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedHistoricalTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "search_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SearchFinishReason = table.Column<int>(type: "INTEGER", nullable: false),
                    SeekedPhrasesJsonList = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "historical_folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AbsolutePath = table.Column<string>(type: "TEXT", nullable: false),
                    SearchLogId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historical_folders_search_logs_SearchLogId",
                        column: x => x.SearchLogId,
                        principalTable: "search_logs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historical_book_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileNameWithExtension = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfPages = table.Column<int>(type: "INTEGER", nullable: false),
                    HistoricalFolderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historical_book_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historical_book_details_historical_folders_HistoricalFolderId",
                        column: x => x.HistoricalFolderId,
                        principalTable: "historical_folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "search_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FoundOnPage = table.Column<int>(type: "INTEGER", nullable: false),
                    Sentence = table.Column<string>(type: "TEXT", nullable: false),
                    HistoricalBookDetailId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_search_results_historical_book_details_HistoricalBookDetailId",
                        column: x => x.HistoricalBookDetailId,
                        principalTable: "historical_book_details",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_historical_book_details_HistoricalFolderId",
                table: "historical_book_details",
                column: "HistoricalFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_historical_folders_SearchLogId",
                table: "historical_folders",
                column: "SearchLogId");

            migrationBuilder.CreateIndex(
                name: "IX_search_results_HistoricalBookDetailId",
                table: "search_results",
                column: "HistoricalBookDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "search_results");

            migrationBuilder.DropTable(
                name: "historical_book_details");

            migrationBuilder.DropTable(
                name: "historical_folders");

            migrationBuilder.DropTable(
                name: "search_logs");
        }
    }
}
