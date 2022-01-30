using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class removeUserInfoInComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar",
                table: "evaluation_comment");

            migrationBuilder.DropColumn(
                name: "nick_name",
                table: "evaluation_comment");

            migrationBuilder.DropColumn(
                name: "replay_nickname",
                table: "evaluation_comment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "evaluation_comment",
                type: "longtext",
                nullable: true,
                comment: "用户头像")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "nick_name",
                table: "evaluation_comment",
                type: "longtext",
                nullable: false,
                comment: "用户名")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "replay_nickname",
                table: "evaluation_comment",
                type: "longtext",
                nullable: true,
                comment: "回复的玩家名")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
