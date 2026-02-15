using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class RenameToGifSource : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "GifSources",
            table: "ChannelSettings",
            newName: "GifSource");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "GifSource",
            table: "ChannelSettings",
            newName: "GifSources");
    }
}
