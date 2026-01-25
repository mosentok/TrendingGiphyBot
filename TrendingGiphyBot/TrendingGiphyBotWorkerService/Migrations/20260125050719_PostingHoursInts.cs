using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class PostingHoursInts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "PostingHoursTo",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "PostingHoursFrom",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "PostingHoursTo",
            table: "ChannelSettings",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "INTEGER",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PostingHoursFrom",
            table: "ChannelSettings",
            type: "TEXT",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "INTEGER",
            oldNullable: true);
    }
}
