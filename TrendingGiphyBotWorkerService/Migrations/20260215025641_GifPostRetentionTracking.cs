using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class GifPostRetentionTracking : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_KlipyPosts_ChannelId",
            table: "KlipyPosts");

        migrationBuilder.DropIndex(
            name: "IX_GiphyPosts_ChannelId",
            table: "GiphyPosts");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedUtc",
            table: "KlipyPosts",
            type: "TEXT",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedUtc",
            table: "GiphyPosts",
            type: "TEXT",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "RetentionDays",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_KlipyPosts_ChannelId_CreatedUtc",
            table: "KlipyPosts",
            columns: new[] { "ChannelId", "CreatedUtc" },
            descending: new[] { false, true });

        migrationBuilder.CreateIndex(
            name: "IX_KlipyPosts_CreatedUtc",
            table: "KlipyPosts",
            column: "CreatedUtc");

        migrationBuilder.CreateIndex(
            name: "IX_GiphyPosts_ChannelId_CreatedUtc",
            table: "GiphyPosts",
            columns: new[] { "ChannelId", "CreatedUtc" },
            descending: new[] { false, true });

        migrationBuilder.CreateIndex(
            name: "IX_GiphyPosts_CreatedUtc",
            table: "GiphyPosts",
            column: "CreatedUtc");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_KlipyPosts_ChannelId_CreatedUtc",
            table: "KlipyPosts");

        migrationBuilder.DropIndex(
            name: "IX_KlipyPosts_CreatedUtc",
            table: "KlipyPosts");

        migrationBuilder.DropIndex(
            name: "IX_GiphyPosts_ChannelId_CreatedUtc",
            table: "GiphyPosts");

        migrationBuilder.DropIndex(
            name: "IX_GiphyPosts_CreatedUtc",
            table: "GiphyPosts");

        migrationBuilder.DropColumn(
            name: "CreatedUtc",
            table: "KlipyPosts");

        migrationBuilder.DropColumn(
            name: "CreatedUtc",
            table: "GiphyPosts");

        migrationBuilder.DropColumn(
            name: "RetentionDays",
            table: "ChannelSettings");

        migrationBuilder.CreateIndex(
            name: "IX_KlipyPosts_ChannelId",
            table: "KlipyPosts",
            column: "ChannelId");

        migrationBuilder.CreateIndex(
            name: "IX_GiphyPosts_ChannelId",
            table: "GiphyPosts",
            column: "ChannelId");
    }
}
