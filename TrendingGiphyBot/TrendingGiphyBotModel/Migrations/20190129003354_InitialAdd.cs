using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrendingGiphyBotModel.Migrations
{
    public partial class InitialAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobConfig",
                columns: table => new
                {
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Interval = table.Column<int>(nullable: true),
                    Time = table.Column<string>(nullable: true),
                    RandomSearchString = table.Column<string>(nullable: true),
                    MinQuietHour = table.Column<short>(nullable: true),
                    MaxQuietHour = table.Column<short>(nullable: true),
                    IntervalMinutes = table.Column<int>(nullable: true),
                    Prefix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobConfig", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "UrlCache",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Stamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrlHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    GifId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Stamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlHistory_JobConfig_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "JobConfig",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_ChannelId",
                table: "UrlHistory",
                column: "ChannelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlCache");

            migrationBuilder.DropTable(
                name: "UrlHistory");

            migrationBuilder.DropTable(
                name: "JobConfig");
        }
    }
}
