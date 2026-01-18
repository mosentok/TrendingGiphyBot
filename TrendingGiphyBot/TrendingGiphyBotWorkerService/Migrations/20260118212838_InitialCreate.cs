using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Intervals",
            columns: table => new
            {
                IntervalId = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Description = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Intervals", x => x.IntervalId);
            });

        migrationBuilder.CreateTable(
            name: "ChannelSettings",
            columns: table => new
            {
                ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                IntervalId = table.Column<int>(type: "INTEGER", nullable: false),
                HowOften = table.Column<string>(type: "TEXT", nullable: true),
                GifPostingBehavior = table.Column<string>(type: "TEXT", nullable: true),
                GifKeyword = table.Column<string>(type: "TEXT", nullable: true),
                PostingHoursFrom = table.Column<string>(type: "TEXT", nullable: true),
                PostingHoursTo = table.Column<string>(type: "TEXT", nullable: true),
                TimeZone = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChannelSettings", x => x.ChannelId);
                table.ForeignKey(
                    name: "FK_ChannelSettings_Intervals_IntervalId",
                    column: x => x.IntervalId,
                    principalTable: "Intervals",
                    principalColumn: "IntervalId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GifPost",
            columns: table => new
            {
                GifPostId = table.Column<Guid>(type: "TEXT", nullable: false),
                ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                GiphyDataId = table.Column<string>(type: "TEXT", nullable: false),
                GifUrl = table.Column<string>(type: "TEXT", nullable: false),
                IsTrending = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GifPost", x => x.GifPostId);
                table.ForeignKey(
                    name: "FK_GifPost_ChannelSettings_ChannelId",
                    column: x => x.ChannelId,
                    principalTable: "ChannelSettings",
                    principalColumn: "ChannelId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChannelSettings_IntervalId",
            table: "ChannelSettings",
            column: "IntervalId");

        migrationBuilder.CreateIndex(
            name: "IX_GifPost_ChannelId",
            table: "GifPost",
            column: "ChannelId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GifPost");

        migrationBuilder.DropTable(
            name: "ChannelSettings");

        migrationBuilder.DropTable(
            name: "Intervals");
    }
}
