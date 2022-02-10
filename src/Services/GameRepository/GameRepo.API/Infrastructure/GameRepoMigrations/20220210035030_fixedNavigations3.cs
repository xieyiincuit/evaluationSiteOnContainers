using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class fixedNavigations3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_owner_game_info_game_id",
                table: "game_owner");

            migrationBuilder.DropForeignKey(
                name: "FK_gameshop_items_game_info_GameInfoId1",
                table: "gameshop_items");

            migrationBuilder.DropIndex(
                name: "IX_gameshop_items_GameInfoId1",
                table: "gameshop_items");

            migrationBuilder.DropIndex(
                name: "IX_game_owner_game_id",
                table: "game_owner");

            migrationBuilder.DropColumn(
                name: "GameInfoId1",
                table: "gameshop_items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameInfoId1",
                table: "gameshop_items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_gameshop_items_GameInfoId1",
                table: "gameshop_items",
                column: "GameInfoId1");

            migrationBuilder.CreateIndex(
                name: "IX_game_owner_game_id",
                table: "game_owner",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "FK_game_owner_game_info_game_id",
                table: "game_owner",
                column: "game_id",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameshop_items_game_info_GameInfoId1",
                table: "gameshop_items",
                column: "GameInfoId1",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
