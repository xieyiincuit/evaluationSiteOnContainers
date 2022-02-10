using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class Concurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConcurrencyToken",
                table: "gameitem_sdk",
                type: "rowversion(6)",
                rowVersion: true,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "game_owner",
                type: "int",
                nullable: false,
                comment: "游戏信息id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "游戏商品id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "gameitem_sdk");

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "game_owner",
                type: "int",
                nullable: false,
                comment: "游戏商品id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "游戏信息id");
        }
    }
}
