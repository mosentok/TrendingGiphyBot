using Microsoft.EntityFrameworkCore.Migrations;

namespace TrendingGiphyBotModel.Migrations
{
    public partial class MoreDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_UrlCache", "UrlCache");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "UrlHistory",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GifId",
                table: "UrlHistory",
                type: "varchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "UrlCache",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UrlCache",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "JobConfig",
                type: "varchar(7)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RandomSearchString",
                table: "JobConfig",
                type: "varchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "JobConfig",
                type: "varchar(4)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "Interval",
                table: "JobConfig",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey("PK_UrlCache", "UrlCache", "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "UrlHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GifId",
                table: "UrlHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "UrlCache",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UrlCache",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "JobConfig",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RandomSearchString",
                table: "JobConfig",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "JobConfig",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Interval",
                table: "JobConfig",
                nullable: true,
                oldClrType: typeof(short),
                oldNullable: true);
        }
    }
}
