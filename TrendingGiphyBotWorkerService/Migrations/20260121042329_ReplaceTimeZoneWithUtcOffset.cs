using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class ReplaceTimeZoneWithUtcOffset : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "TimeZone",
            table: "ChannelSettings",
            newName: "UtcOffset");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UtcOffset",
            table: "ChannelSettings",
            newName: "TimeZone");
    }
}
