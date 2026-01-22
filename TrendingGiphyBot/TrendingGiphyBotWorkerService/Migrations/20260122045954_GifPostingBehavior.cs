using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations;

/// <inheritdoc />
public partial class GifPostingBehavior : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GifPostingBehavior",
            table: "ChannelSettings");

        migrationBuilder.AddColumn<int>(
            name: "GifPostingBehaviorId",
            table: "ChannelSettings",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "GifPostingBehaviors",
            columns: table => new
            {
                GifPostingBehaviorId = table.Column<int>(type: "INTEGER", nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GifPostingBehaviors", x => x.GifPostingBehaviorId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChannelSettings_GifPostingBehaviorId",
            table: "ChannelSettings",
            column: "GifPostingBehaviorId");

        migrationBuilder.AddForeignKey(
            name: "FK_ChannelSettings_GifPostingBehaviors_GifPostingBehaviorId",
            table: "ChannelSettings",
            column: "GifPostingBehaviorId",
            principalTable: "GifPostingBehaviors",
            principalColumn: "GifPostingBehaviorId",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ChannelSettings_GifPostingBehaviors_GifPostingBehaviorId",
            table: "ChannelSettings");

        migrationBuilder.DropTable(
            name: "GifPostingBehaviors");

        migrationBuilder.DropIndex(
            name: "IX_ChannelSettings_GifPostingBehaviorId",
            table: "ChannelSettings");

        migrationBuilder.DropColumn(
            name: "GifPostingBehaviorId",
            table: "ChannelSettings");

        migrationBuilder.AddColumn<string>(
            name: "GifPostingBehavior",
            table: "ChannelSettings",
            type: "TEXT",
            nullable: true);
    }
}
