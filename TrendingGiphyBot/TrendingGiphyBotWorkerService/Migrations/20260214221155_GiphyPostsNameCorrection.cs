using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class GiphyPostsNameCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GifPosts_ChannelSettings_ChannelId",
                table: "GifPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GifPosts",
                table: "GifPosts");

            migrationBuilder.RenameTable(
                name: "GifPosts",
                newName: "GiphyPosts");

            migrationBuilder.RenameIndex(
                name: "IX_GifPosts_ChannelId",
                table: "GiphyPosts",
                newName: "IX_GiphyPosts_ChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GiphyPosts",
                table: "GiphyPosts",
                column: "GiphyPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiphyPosts_ChannelSettings_ChannelId",
                table: "GiphyPosts",
                column: "ChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiphyPosts_ChannelSettings_ChannelId",
                table: "GiphyPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GiphyPosts",
                table: "GiphyPosts");

            migrationBuilder.RenameTable(
                name: "GiphyPosts",
                newName: "GifPosts");

            migrationBuilder.RenameIndex(
                name: "IX_GiphyPosts_ChannelId",
                table: "GifPosts",
                newName: "IX_GifPosts_ChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GifPosts",
                table: "GifPosts",
                column: "GiphyPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_GifPosts_ChannelSettings_ChannelId",
                table: "GifPosts",
                column: "ChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
