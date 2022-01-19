using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class UpdateRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info");

            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                table: "game_info",
                type: "int",
                nullable: true,
                comment: "游戏公司外键",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "game_info",
                type: "int",
                nullable: false,
                comment: "游戏类别外键",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "play_suggestion_id",
                table: "game_info",
                type: "int",
                nullable: true,
                comment: "游戏游玩建议外键");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info",
                column: "company_id",
                principalTable: "game_company",
                principalColumn: "company_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info");

            migrationBuilder.DropColumn(
                name: "play_suggestion_id",
                table: "game_info");

            migrationBuilder.AlterColumn<int>(
                name: "company_id",
                table: "game_info",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "游戏公司外键");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "game_info",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "游戏类别外键");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info",
                column: "company_id",
                principalTable: "game_company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
