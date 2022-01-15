using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class addRootCommentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "root_comment_id",
                table: "evaluation_comment",
                type: "int",
                nullable: true,
                comment: "回复评论属于哪个主评论");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "root_comment_id",
                table: "evaluation_comment");
        }
    }
}
