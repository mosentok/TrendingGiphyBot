using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelSettings",
                columns: table => new
                {
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    GifPostingBehavior = table.Column<int>(type: "INTEGER", nullable: false),
                    GifKeyword = table.Column<string>(type: "TEXT", nullable: true),
                    GiphyRating = table.Column<string>(type: "TEXT", nullable: true),
                    GifSource = table.Column<int>(type: "INTEGER", nullable: true),
                    Interval = table.Column<int>(type: "INTEGER", nullable: false),
                    PostingHoursFrom = table.Column<int>(type: "INTEGER", nullable: true),
                    PostingHoursTo = table.Column<int>(type: "INTEGER", nullable: true),
                    RetentionDays = table.Column<int>(type: "INTEGER", nullable: true),
                    UtcOffset = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSettings", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "GiphyPosts",
                columns: table => new
                {
                    GiphyPostId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GiphyDataId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiphyPosts", x => x.GiphyPostId);
                    table.ForeignKey(
                        name: "FK_GiphyPosts_ChannelSettings_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "ChannelSettings",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KlipyPosts",
                columns: table => new
                {
                    KlipyPostId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    KlipyDataId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KlipyPosts", x => x.KlipyPostId);
                    table.ForeignKey(
                        name: "FK_KlipyPosts_ChannelSettings_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "ChannelSettings",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiphyPosts_ChannelId_CreatedUtc",
                table: "GiphyPosts",
                columns: new[] { "ChannelId", "CreatedUtc" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_GiphyPosts_CreatedUtc",
                table: "GiphyPosts",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_KlipyPosts_ChannelId_CreatedUtc",
                table: "KlipyPosts",
                columns: new[] { "ChannelId", "CreatedUtc" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_KlipyPosts_CreatedUtc",
                table: "KlipyPosts",
                column: "CreatedUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiphyPosts");

            migrationBuilder.DropTable(
                name: "KlipyPosts");

            migrationBuilder.DropTable(
                name: "ChannelSettings");
        }
    }
}
