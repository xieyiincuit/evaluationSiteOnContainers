using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class Initalize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evaluation_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "测评类别主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "测评类别名")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_category", x => x.id);
                },
                comment: "测评文章分类表");

            migrationBuilder.CreateTable(
                name: "evaluation_article",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "测评内容作者"),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "测评文章标题"),
                    desciption_image = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "展示略缩图"),
                    article_image = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "内容Top呈现图"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "测评文章内容"),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "测评内容创建时间"),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "测评内容更新时间"),
                    traffic = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章浏览量"),
                    comment_nums = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章评论数量"),
                    like_nums = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章点赞数量"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, comment: "逻辑删除"),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "文章测评简介"),
                    article_status = table.Column<int>(type: "int", nullable: false, comment: "文章发布状态"),
                    category_type_id = table.Column<int>(type: "int", nullable: false, comment: "测评类别主键"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "测评内容关联游戏id"),
                    game_name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "测评内容关联游戏名")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_article", x => x.id);
                    table.ForeignKey(
                        name: "foreignKey_type_article",
                        column: x => x.category_type_id,
                        principalTable: "evaluation_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "游戏测评文章信息表");

            migrationBuilder.CreateTable(
                name: "evaluation_comment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "评论主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "评论内容"),
                    like_nums = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "评论点赞数量"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "用户名"),
                    user_avatar = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "用户头像"),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "评论时间"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, comment: "逻辑删除"),
                    is_replay = table.Column<bool>(type: "bit", nullable: true, comment: "该评论是否为回复"),
                    replay_id = table.Column<int>(type: "int", nullable: true, comment: "回复的评论id"),
                    article_id = table.Column<int>(type: "int", nullable: false, comment: "评论对应的测评id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_comment", x => x.id);
                    table.ForeignKey(
                        name: "foreignKey_comment_article",
                        column: x => x.article_id,
                        principalTable: "evaluation_article",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "测评文章评论表");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_article_category_type_id",
                table: "evaluation_article",
                column: "category_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_comment_article_id",
                table: "evaluation_comment",
                column: "article_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evaluation_comment");

            migrationBuilder.DropTable(
                name: "evaluation_article");

            migrationBuilder.DropTable(
                name: "evaluation_category");
        }
    }
}
