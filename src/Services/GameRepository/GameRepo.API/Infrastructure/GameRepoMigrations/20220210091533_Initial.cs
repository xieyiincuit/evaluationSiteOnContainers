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
                name: "game_owner",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "游戏信息id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_owner", x => new { x.user_id, x.game_id });
                },
                comment: "玩家游戏拥有表")
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
                        name: "FK_game_info_game_type_category_id",
                        column: x => x.category_id,
                        principalTable: "game_type",
                        principalColumn: "type_id");
                },
                comment: "游戏信息表")
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
                    table.ForeignKey(
                        name: "FK_game_playsuggestion_game_info_game_id",
                        column: x => x.game_id,
                        principalTable: "game_info",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "游玩游戏配置建议表")
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

            migrationBuilder.CreateTable(
                name: "gameshop_items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false, comment: "售价"),
                    discount = table.Column<float>(type: "float", nullable: false, comment: "折扣"),
                    available_stock = table.Column<int>(type: "int", nullable: false, comment: "库存量"),
                    hotsell_point = table.Column<double>(type: "double", nullable: false, defaultValue: 10.0, comment: "销售热度"),
                    temporary_stopsell = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "暂停销售"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "游戏信息外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameshop_items", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_gameshop_items_game_info_game_id",
                        column: x => x.game_id,
                        principalTable: "game_info",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "游戏售卖商品表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gameitem_sdk",
                columns: table => new
                {
                    sdk_id = table.Column<long>(type: "bigint", nullable: false, comment: "SDK主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sdk_string = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false, comment: "游戏SDK码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    has_send = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "是否已卖出"),
                    sent_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "发送时间"),
                    item_id = table.Column<int>(type: "int", nullable: false, comment: "游戏商品外键"),
                    row_version = table.Column<DateTime>(type: "datetime(6)", rowVersion: true, nullable: false, comment: "行版本")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameitem_sdk", x => x.sdk_id);
                    table.ForeignKey(
                        name: "FK_gameitem_sdk_gameshop_items_item_id",
                        column: x => x.item_id,
                        principalTable: "gameshop_items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "sdk存放表, 负责给玩家发送未发送的sdk")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gamesdk_player",
                columns: table => new
                {
                    sdk_player_id = table.Column<int>(type: "int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "购买用户的Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    has_checked = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "是否已经被校验"),
                    check_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "校验的时间"),
                    sdk_id = table.Column<long>(type: "bigint", nullable: false, comment: "游戏发放的sdk外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamesdk_player", x => x.sdk_player_id);
                    table.ForeignKey(
                        name: "FK_gamesdk_player_gameitem_sdk_sdk_id",
                        column: x => x.sdk_id,
                        principalTable: "gameitem_sdk",
                        principalColumn: "sdk_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "保存用户购买的SDK信息")
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
                name: "IX_game_playsuggestion_game_id",
                table: "game_playsuggestion",
                column: "game_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gameinfo_tag_tag_id",
                table: "gameinfo_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_gameitem_sdk_item_id",
                table: "gameitem_sdk",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_gamesdk_player_sdk_id",
                table: "gamesdk_player",
                column: "sdk_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gamesdk_player_user_id",
                table: "gamesdk_player",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_gameshop_items_game_id",
                table: "gameshop_items",
                column: "game_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_owner");

            migrationBuilder.DropTable(
                name: "game_playsuggestion");

            migrationBuilder.DropTable(
                name: "gameinfo_tag");

            migrationBuilder.DropTable(
                name: "gamesdk_player");

            migrationBuilder.DropTable(
                name: "game_tag");

            migrationBuilder.DropTable(
                name: "gameitem_sdk");

            migrationBuilder.DropTable(
                name: "gameshop_items");

            migrationBuilder.DropTable(
                name: "game_info");

            migrationBuilder.DropTable(
                name: "game_company");

            migrationBuilder.DropTable(
                name: "game_type");
        }
    }
}
