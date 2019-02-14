using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrendingGiphyBotModel.Migrations
{
    public partial class ChannelIdIndexIncludeGifId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlHistory_ChannelId",
                table: "UrlHistory");

            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_ChannelId",
                table: "UrlHistory",
                column: "ChannelId")
                .Annotation("SqlServer:Include", new[] { "GifId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlHistory_ChannelId",
                table: "UrlHistory");

            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_ChannelId",
                table: "UrlHistory",
                column: "ChannelId");
        }
    }
}
