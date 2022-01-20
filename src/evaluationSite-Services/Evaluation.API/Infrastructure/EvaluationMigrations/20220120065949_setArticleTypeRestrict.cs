using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    public partial class setArticleTypeRestrict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "foreignKey_type_article",
                table: "evaluation_article");

            migrationBuilder.AddForeignKey(
                name: "foreignKey_type_article",
                table: "evaluation_article",
                column: "category_type_id",
                principalTable: "evaluation_category",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "foreignKey_type_article",
                table: "evaluation_article");

            migrationBuilder.AddForeignKey(
                name: "foreignKey_type_article",
                table: "evaluation_article",
                column: "category_type_id",
                principalTable: "evaluation_category",
                principalColumn: "category_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
