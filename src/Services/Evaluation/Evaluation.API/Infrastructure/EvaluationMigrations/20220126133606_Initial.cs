using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "evaluation_category",
                table => new
                {
                    category_id = table.Column<int>("int", nullable: false, comment: "测评类别主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_type = table
                        .Column<string>("varchar(10)", maxLength: 10, nullable: false, comment: "测评类别名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, defaultValue: false, comment: "逻辑删除")
                },
                constraints: table => { table.PrimaryKey("PK_evaluation_category", x => x.category_id); },
                comment: "测评文章分类表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "evaluation_article",
                table => new
                {
                    article_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>("varchar(450)", maxLength: 450, nullable: false, comment: "测评内容作者id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_name = table.Column<string>("varchar(100)", maxLength: 100, nullable: false,
                            comment: "测评内容作者姓名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "测评文章标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description_image = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "展示略缩图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    article_image = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "内容Top呈现图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>("longtext", nullable: false, comment: "测评文章内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>("datetime(6)", nullable: false, comment: "测评内容创建时间"),
                    update_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "测评内容更新时间"),
                    join_count = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "文章浏览量"),
                    comments_count = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "文章评论数量"),
                    support_count = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "文章点赞数量"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, comment: "逻辑删除"),
                    description = table
                        .Column<string>("varchar(100)", maxLength: 100, nullable: true, comment: "文章测评简介")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    article_status = table.Column<int>("int", nullable: false, comment: "文章发布状态"),
                    category_type_id = table.Column<int>("int", nullable: true, comment: "测评类别主键"),
                    game_id = table.Column<int>("int", nullable: false, comment: "测评内容关联游戏id"),
                    game_name = table.Column<string>("varchar(100)", maxLength: 100, nullable: false,
                            comment: "测评内容关联游戏名")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_article", x => x.article_id);
                    table.ForeignKey(
                        "foreignKey_type_article",
                        x => x.category_type_id,
                        "evaluation_category",
                        "category_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "游戏测评文章信息表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "evaluation_comment",
                table => new
                {
                    comment_id = table.Column<int>("int", nullable: false, comment: "评论主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    content = table.Column<string>("varchar(500)", maxLength: 500, nullable: false, comment: "评论内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    support_count = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "评论点赞数量"),
                    user_id = table.Column<string>("varchar(450)", maxLength: 450, nullable: false, comment: "用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nick_name = table.Column<string>("longtext", nullable: false, comment: "用户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    avatar = table.Column<string>("longtext", nullable: true, comment: "用户头像")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>("datetime(6)", nullable: false, comment: "评论时间"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, comment: "逻辑删除"),
                    is_replay = table.Column<bool>("tinyint(1)", nullable: true, comment: "该评论是否为回复"),
                    replay_comment_id = table.Column<int>("int", nullable: true, comment: "回复的评论id"),
                    replay_userid = table
                        .Column<string>("varchar(450)", maxLength: 450, nullable: true, comment: "回复的玩家Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    replay_nickname = table.Column<string>("longtext", nullable: true, comment: "回复的玩家名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    root_comment_id = table.Column<int>("int", nullable: true, comment: "回复评论属于哪个主评论"),
                    article_id = table.Column<int>("int", nullable: false, comment: "评论对应的测评id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_comment", x => x.comment_id);
                    table.ForeignKey(
                        "foreignKey_comment_article",
                        x => x.article_id,
                        "evaluation_article",
                        "article_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "测评文章评论表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            "IX_evaluation_article_category_type_id",
            "evaluation_article",
            "category_type_id");

        migrationBuilder.CreateIndex(
            "IX_evaluation_article_user_id",
            "evaluation_article",
            "user_id");

        migrationBuilder.CreateIndex(
            "IX_evaluation_comment_article_id",
            "evaluation_comment",
            "article_id");

        migrationBuilder.CreateIndex(
            "IX_evaluation_comment_root_comment_id",
            "evaluation_comment",
            "root_comment_id");

        migrationBuilder.CreateIndex(
            "IX_evaluation_comment_user_id",
            "evaluation_comment",
            "user_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "evaluation_comment");

        migrationBuilder.DropTable(
            "evaluation_article");

        migrationBuilder.DropTable(
            "evaluation_category");
    }
}