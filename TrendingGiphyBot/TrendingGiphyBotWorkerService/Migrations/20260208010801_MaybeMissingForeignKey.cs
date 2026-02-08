using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class MaybeMissingForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KlipyPosts_ChannelSettings_ChannelSettingsChannelId",
                table: "KlipyPosts");

            migrationBuilder.DropIndex(
                name: "IX_KlipyPosts_ChannelSettingsChannelId",
                table: "KlipyPosts");

            migrationBuilder.DropColumn(
                name: "ChannelSettingsChannelId",
                table: "KlipyPosts");

            migrationBuilder.CreateIndex(
                name: "IX_KlipyPosts_ChannelId",
                table: "KlipyPosts",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_KlipyPosts_ChannelSettings_ChannelId",
                table: "KlipyPosts",
                column: "ChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KlipyPosts_ChannelSettings_ChannelId",
                table: "KlipyPosts");

            migrationBuilder.DropIndex(
                name: "IX_KlipyPosts_ChannelId",
                table: "KlipyPosts");

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelSettingsChannelId",
                table: "KlipyPosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_KlipyPosts_ChannelSettingsChannelId",
                table: "KlipyPosts",
                column: "ChannelSettingsChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_KlipyPosts_ChannelSettings_ChannelSettingsChannelId",
                table: "KlipyPosts",
                column: "ChannelSettingsChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
