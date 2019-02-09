using Microsoft.EntityFrameworkCore.Migrations;

namespace TrendingGiphyBotModel.Migrations
{
    public partial class HistoryStampIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_Stamp",
                table: "UrlHistory",
                column: "Stamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlHistory_Stamp",
                table: "UrlHistory");
        }
    }
}
