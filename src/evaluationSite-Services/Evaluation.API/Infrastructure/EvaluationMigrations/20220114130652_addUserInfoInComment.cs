using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EntityConfigurations
{
    public partial class addUserInfoInComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "replay_id",
                table: "evaluation_comment",
                newName: "replay_comment_id");

            migrationBuilder.AddColumn<string>(
                name: "replay_nickname",
                table: "evaluation_comment",
                type: "nvarchar(max)",
                nullable: true,
                comment: "回复的玩家名");

            migrationBuilder.AddColumn<int>(
                name: "replay_userid",
                table: "evaluation_comment",
                type: "int",
                nullable: true,
                comment: "回复的玩家Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "replay_nickname",
                table: "evaluation_comment");

            migrationBuilder.DropColumn(
                name: "replay_userid",
                table: "evaluation_comment");

            migrationBuilder.RenameColumn(
                name: "replay_comment_id",
                table: "evaluation_comment",
                newName: "replay_id");
        }
    }
}
