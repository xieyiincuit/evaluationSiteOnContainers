using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evaluation_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false, comment: "测评类别主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "测评类别名"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false, comment: "逻辑删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_category", x => x.category_id);
                },
                comment: "测评文章分类表");

            migrationBuilder.CreateTable(
                name: "evaluation_article",
                columns: table => new
                {
                    article_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "测评内容作者id"),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "测评文章标题"),
                    desciption_image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "展示略缩图"),
                    article_image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "内容Top呈现图"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "测评文章内容"),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "测评内容创建时间"),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "测评内容更新时间"),
                    join_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章浏览量"),
                    comments_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章评论数量"),
                    support_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "文章点赞数量"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, comment: "逻辑删除"),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "文章测评简介"),
                    article_status = table.Column<int>(type: "int", nullable: false, comment: "文章发布状态"),
                    category_type_id = table.Column<int>(type: "int", nullable: true, comment: "测评类别主键"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "测评内容关联游戏id"),
                    game_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "测评内容关联游戏名")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_article", x => x.article_id);
                    table.ForeignKey(
                        name: "foreignKey_type_article",
                        column: x => x.category_type_id,
                        principalTable: "evaluation_category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.SetNull);
                },
                comment: "游戏测评文章信息表");

            migrationBuilder.CreateTable(
                name: "evaluation_comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false, comment: "评论主键")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "评论内容"),
                    support_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "评论点赞数量"),
                    user_id = table.Column<int>(type: "int", nullable: false, comment: "用户id"),
                    nick_name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "用户名"),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "用户头像"),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "评论时间"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, comment: "逻辑删除"),
                    is_replay = table.Column<bool>(type: "bit", nullable: true, comment: "该评论是否为回复"),
                    replay_comment_id = table.Column<int>(type: "int", nullable: true, comment: "回复的评论id"),
                    replay_userid = table.Column<int>(type: "int", nullable: true, comment: "回复的玩家Id"),
                    replay_nickname = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "回复的玩家名"),
                    root_comment_id = table.Column<int>(type: "int", nullable: true, comment: "回复评论属于哪个主评论"),
                    article_id = table.Column<int>(type: "int", nullable: false, comment: "评论对应的测评id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "foreignKey_comment_article",
                        column: x => x.article_id,
                        principalTable: "evaluation_article",
                        principalColumn: "article_id",
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
