using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class fixedNavigations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "gamesdk_player",
                type: "varchar(255)",
                nullable: false,
                comment: "购买用户的Id",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "购买用户的Id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_gamesdk_player_user_id",
                table: "gamesdk_player",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_gamesdk_player_user_id",
                table: "gamesdk_player");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "gamesdk_player",
                type: "longtext",
                nullable: false,
                comment: "购买用户的Id",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "购买用户的Id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
