using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class fixedNavigations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_gameshop_items_game_info_GameInfoId1",
                table: "gameshop_items",
                column: "GameInfoId1",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gameshop_items_game_info_GameInfoId1",
                table: "gameshop_items");

            migrationBuilder.DropIndex(
                name: "IX_gameshop_items_GameInfoId1",
                table: "gameshop_items");

            migrationBuilder.DropColumn(
                name: "GameInfoId1",
                table: "gameshop_items");
        }
    }
}
