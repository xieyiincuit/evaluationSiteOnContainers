using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class relationChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_playsuggestion_game_playsuggestion_id",
                table: "game_info");

            migrationBuilder.DropIndex(
                name: "IX_game_info_game_playsuggestion_id",
                table: "game_info");

            migrationBuilder.CreateIndex(
                name: "IX_game_playsuggestion_game_id",
                table: "game_playsuggestion",
                column: "game_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_game_playsuggestion_game_info_game_id",
                table: "game_playsuggestion",
                column: "game_id",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_playsuggestion_game_info_game_id",
                table: "game_playsuggestion");

            migrationBuilder.DropIndex(
                name: "IX_game_playsuggestion_game_id",
                table: "game_playsuggestion");

            migrationBuilder.CreateIndex(
                name: "IX_game_info_game_playsuggestion_id",
                table: "game_info",
                column: "game_playsuggestion_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_playsuggestion_game_playsuggestion_id",
                table: "game_info",
                column: "game_playsuggestion_id",
                principalTable: "game_playsuggestion",
                principalColumn: "suggestion_id");
        }
    }
}
