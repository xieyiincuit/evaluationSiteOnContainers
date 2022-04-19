using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class articleLike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evaluation_like",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    article_id = table.Column<int>(type: "int", nullable: false, comment: "文章id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2022, 4, 19, 21, 8, 25, 351, DateTimeKind.Local).AddTicks(290), comment: "点赞时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation_like", x => x.Id);
                    table.ForeignKey(
                        name: "foreignKey_likeRecord_article",
                        column: x => x.article_id,
                        principalTable: "evaluation_article",
                        principalColumn: "article_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "测评文章点赞记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_like_article_id",
                table: "evaluation_like",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_like_user_id",
                table: "evaluation_like",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evaluation_like");
        }
    }
}
