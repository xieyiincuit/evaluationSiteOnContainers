using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class ChangeModelsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "evaluation_comment",
                newName: "nick_name");

            migrationBuilder.RenameColumn(
                name: "user_avatar",
                table: "evaluation_comment",
                newName: "avatar");

            migrationBuilder.RenameColumn(
                name: "like_nums",
                table: "evaluation_comment",
                newName: "support_count");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "evaluation_comment",
                newName: "comment_id");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "evaluation_category",
                newName: "category_type");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "evaluation_category",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "traffic",
                table: "evaluation_article",
                newName: "join_count");

            migrationBuilder.RenameColumn(
                name: "like_nums",
                table: "evaluation_article",
                newName: "support_count");

            migrationBuilder.RenameColumn(
                name: "comment_nums",
                table: "evaluation_article",
                newName: "comments_count");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "evaluation_article",
                newName: "article_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "support_count",
                table: "evaluation_comment",
                newName: "like_nums");

            migrationBuilder.RenameColumn(
                name: "nick_name",
                table: "evaluation_comment",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "evaluation_comment",
                newName: "user_avatar");

            migrationBuilder.RenameColumn(
                name: "comment_id",
                table: "evaluation_comment",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "category_type",
                table: "evaluation_category",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "evaluation_category",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "support_count",
                table: "evaluation_article",
                newName: "like_nums");

            migrationBuilder.RenameColumn(
                name: "join_count",
                table: "evaluation_article",
                newName: "traffic");

            migrationBuilder.RenameColumn(
                name: "comments_count",
                table: "evaluation_article",
                newName: "comment_nums");

            migrationBuilder.RenameColumn(
                name: "article_id",
                table: "evaluation_article",
                newName: "id");
        }
    }
}
