using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class AddGifSources : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "GifSources",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.Sql("UPDATE ChannelSettings SET GifSources = 1 WHERE GifSources IS NULL;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GifSources",
            table: "ChannelSettings");
    }
}
