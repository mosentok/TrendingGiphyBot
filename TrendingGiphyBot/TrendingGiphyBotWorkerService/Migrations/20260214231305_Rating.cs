using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendingGiphyBotWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class Rating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GiphyRating",
                table: "ChannelSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiphyRating",
                table: "ChannelSettings");
        }
    }
}
