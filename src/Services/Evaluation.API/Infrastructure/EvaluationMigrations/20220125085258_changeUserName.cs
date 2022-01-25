using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class changeUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "evaluation_comment",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                comment: "用户id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "用户id")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "replay_userid",
                table: "evaluation_comment",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true,
                comment: "回复的玩家Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "回复的玩家Id")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "evaluation_article",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                comment: "测评内容作者id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "测评内容作者id")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_comment_root_comment_id",
                table: "evaluation_comment",
                column: "root_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_comment_user_id",
                table: "evaluation_comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_article_user_id",
                table: "evaluation_article",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_evaluation_comment_root_comment_id",
                table: "evaluation_comment");

            migrationBuilder.DropIndex(
                name: "IX_evaluation_comment_user_id",
                table: "evaluation_comment");

            migrationBuilder.DropIndex(
                name: "IX_evaluation_article_user_id",
                table: "evaluation_article");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "evaluation_comment",
                type: "int",
                nullable: false,
                comment: "用户id",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450,
                oldComment: "用户id")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "replay_userid",
                table: "evaluation_comment",
                type: "int",
                nullable: true,
                comment: "回复的玩家Id",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450,
                oldNullable: true,
                oldComment: "回复的玩家Id")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "evaluation_article",
                type: "int",
                nullable: false,
                comment: "测评内容作者id",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450,
                oldComment: "测评内容作者id")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
