using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class gameIdInSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_play_suggestion_game_info_game_id",
                table: "play_suggestion");

            migrationBuilder.DropIndex(
                name: "IX_play_suggestion_game_id",
                table: "play_suggestion");

            migrationBuilder.DropColumn(
                name: "game_id",
                table: "play_suggestion");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "play_suggestion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "游戏外键id");

            migrationBuilder.CreateIndex(
                name: "IX_game_info_play_suggestion_id",
                table: "game_info",
                column: "play_suggestion_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_play_suggestion_play_suggestion_id",
                table: "game_info",
                column: "play_suggestion_id",
                principalTable: "play_suggestion",
                principalColumn: "suggestion_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_play_suggestion_play_suggestion_id",
                table: "game_info");

            migrationBuilder.DropIndex(
                name: "IX_game_info_play_suggestion_id",
                table: "game_info");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "play_suggestion");

            migrationBuilder.AddColumn<int>(
                name: "game_id",
                table: "play_suggestion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_play_suggestion_game_id",
                table: "play_suggestion",
                column: "game_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_play_suggestion_game_info_game_id",
                table: "play_suggestion",
                column: "game_id",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
