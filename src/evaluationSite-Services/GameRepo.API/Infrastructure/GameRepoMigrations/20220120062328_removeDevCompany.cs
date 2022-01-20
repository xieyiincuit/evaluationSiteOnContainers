using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class removeDevCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info");

            migrationBuilder.DropColumn(
                name: "dev_company",
                table: "game_info");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "game_info",
                type: "int",
                nullable: true,
                comment: "游戏类别外键",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "游戏类别外键");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info",
                column: "category_id",
                principalTable: "game_type",
                principalColumn: "type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "game_info",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "游戏类别外键",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "游戏类别外键");

            migrationBuilder.AddColumn<string>(
                name: "dev_company",
                table: "game_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "开发公司")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info",
                column: "category_id",
                principalTable: "game_type",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
