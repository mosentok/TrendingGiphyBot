using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class GifSources : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "GifPostId",
            table: "GifPosts",
            newName: "GiphyPostId");

        migrationBuilder.AddColumn<int>(
            name: "GifSources",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "KlipyPosts",
            columns: table => new
            {
                KlipyPostId = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                KlipyDataId = table.Column<ulong>(type: "INTEGER", nullable: false),
                ChannelSettingsChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KlipyPosts", x => x.KlipyPostId);
                table.ForeignKey(
                    name: "FK_KlipyPosts_ChannelSettings_ChannelSettingsChannelId",
                    column: x => x.ChannelSettingsChannelId,
                    principalTable: "ChannelSettings",
                    principalColumn: "ChannelId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_KlipyPosts_ChannelSettingsChannelId",
            table: "KlipyPosts",
            column: "ChannelSettingsChannelId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "KlipyPosts");

        migrationBuilder.DropColumn(
            name: "GifSources",
            table: "ChannelSettings");

        migrationBuilder.RenameColumn(
            name: "GiphyPostId",
            table: "GifPosts",
            newName: "GifPostId");
    }
}
