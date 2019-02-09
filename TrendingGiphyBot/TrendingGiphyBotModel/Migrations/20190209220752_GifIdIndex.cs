using Microsoft.EntityFrameworkCore.Migrations;

namespace TrendingGiphyBotModel.Migrations
{
    public partial class GifIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_GifId",
                table: "UrlHistory",
                column: "GifId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlHistory_GifId",
                table: "UrlHistory");
        }
    }
}
