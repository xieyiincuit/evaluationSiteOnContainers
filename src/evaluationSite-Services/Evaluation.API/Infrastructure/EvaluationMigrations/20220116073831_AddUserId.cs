using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class AddUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "author",
                table: "evaluation_article");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "evaluation_article",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "测评内容作者id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "evaluation_article");

            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "evaluation_article",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "测评内容作者");
        }
    }
}
