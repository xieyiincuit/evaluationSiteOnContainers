using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "game_company",
                columns: table => new
                {
                    company_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    company_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "游戏类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_company", x => x.company_id);
                },
                comment: "发行公司")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "game_playsuggestion",
                columns: table => new
                {
                    suggestion_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    operation_system = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "操作系统建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpu_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "CPU型号建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    memory_size = table.Column<double>(type: "double", nullable: false, comment: "内存大小建议"),
                    disk_size = table.Column<double>(type: "double", nullable: false, comment: "磁盘大小建议"),
                    graphics_card = table.Column<string>(type: "longtext", nullable: false, comment: "显卡型号建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "游戏外键id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_playsuggestion", x => x.suggestion_id);
                },
                comment: "游玩游戏配置建议表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "game_tag",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tag_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "游戏标签名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_tag", x => x.tag_id);
                },
                comment: "游戏标签表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "game_type",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "游戏类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_type", x => x.type_id);
                },
                comment: "游戏类型分类表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "game_info",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "游戏名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "游戏描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    details_picture = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "游戏展示图大图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rough_picture = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "游戏展示图小图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    average_score = table.Column<double>(type: "double", nullable: true, comment: "游戏评分"),
                    sell_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "发售时间"),
                    support_platform = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "游玩平台")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hot_points = table.Column<long>(type: "bigint", nullable: true, comment: "游戏热度"),
                    company_id = table.Column<int>(type: "int", nullable: true, comment: "游戏公司外键"),
                    category_id = table.Column<int>(type: "int", nullable: true, comment: "游戏类别外键"),
                    game_playsuggestion_id = table.Column<int>(type: "int", nullable: true, comment: "游戏游玩建议外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_info", x => x.game_id);
                    table.ForeignKey(
                        name: "FK_game_info_game_company_company_id",
                        column: x => x.company_id,
                        principalTable: "game_company",
                        principalColumn: "company_id");
                    table.ForeignKey(
                        name: "FK_game_info_game_playsuggestion_game_playsuggestion_id",
                        column: x => x.game_playsuggestion_id,
                        principalTable: "game_playsuggestion",
                        principalColumn: "suggestion_id");
                    table.ForeignKey(
                        name: "FK_game_info_game_type_category_id",
                        column: x => x.category_id,
                        principalTable: "game_type",
                        principalColumn: "type_id");
                },
                comment: "游戏信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gameinfo_tag",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "游戏id"),
                    tag_id = table.Column<int>(type: "int", nullable: false, comment: "标签id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameinfo_tag", x => new { x.game_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_gameinfo_tag_game_info_game_id",
                        column: x => x.game_id,
                        principalTable: "game_info",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gameinfo_tag_game_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "game_tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "游戏与标签的多对多链接表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_game_info_category_id",
                table: "game_info",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_info_company_id",
                table: "game_info",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_info_game_playsuggestion_id",
                table: "game_info",
                column: "game_playsuggestion_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gameinfo_tag_tag_id",
                table: "gameinfo_tag",
                column: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gameinfo_tag");

            migrationBuilder.DropTable(
                name: "game_info");

            migrationBuilder.DropTable(
                name: "game_tag");

            migrationBuilder.DropTable(
                name: "game_company");

            migrationBuilder.DropTable(
                name: "game_playsuggestion");

            migrationBuilder.DropTable(
                name: "game_type");
        }
    }
}
