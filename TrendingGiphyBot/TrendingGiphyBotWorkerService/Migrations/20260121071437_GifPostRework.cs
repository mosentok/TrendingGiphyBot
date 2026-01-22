using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class GifPostRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GifPost_ChannelSettings_ChannelId",
                table: "GifPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GifPost",
                table: "GifPost");

            migrationBuilder.DropColumn(
                name: "GifUrl",
                table: "GifPost");

            migrationBuilder.DropColumn(
                name: "IsTrending",
                table: "GifPost");

            migrationBuilder.RenameTable(
                name: "GifPost",
                newName: "GifPosts");

            migrationBuilder.RenameIndex(
                name: "IX_GifPost_ChannelId",
                table: "GifPosts",
                newName: "IX_GifPosts_ChannelId");

            migrationBuilder.AlterColumn<long>(
                name: "GifPostId",
                table: "GifPosts",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GifPosts",
                table: "GifPosts",
                column: "GifPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_GifPosts_ChannelSettings_ChannelId",
                table: "GifPosts",
                column: "ChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GifPosts_ChannelSettings_ChannelId",
                table: "GifPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GifPosts",
                table: "GifPosts");

            migrationBuilder.RenameTable(
                name: "GifPosts",
                newName: "GifPost");

            migrationBuilder.RenameIndex(
                name: "IX_GifPosts_ChannelId",
                table: "GifPost",
                newName: "IX_GifPost_ChannelId");

            migrationBuilder.AlterColumn<Guid>(
                name: "GifPostId",
                table: "GifPost",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "GifUrl",
                table: "GifPost",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsTrending",
                table: "GifPost",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GifPost",
                table: "GifPost",
                column: "GifPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_GifPost_ChannelSettings_ChannelId",
                table: "GifPost",
                column: "ChannelId",
                principalTable: "ChannelSettings",
                principalColumn: "ChannelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
