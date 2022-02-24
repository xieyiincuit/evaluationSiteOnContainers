using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations;

public partial class removeUserInfoInComment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "avatar",
            "evaluation_comment");

        migrationBuilder.DropColumn(
            "nick_name",
            "evaluation_comment");

        migrationBuilder.DropColumn(
            "replay_nickname",
            "evaluation_comment");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
                "avatar",
                "evaluation_comment",
                "longtext",
                nullable: true,
                comment: "用户头像")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
                "nick_name",
                "evaluation_comment",
                "longtext",
                nullable: false,
                comment: "用户名")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
                "replay_nickname",
                "evaluation_comment",
                "longtext",
                nullable: true,
                comment: "回复的玩家名")
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}