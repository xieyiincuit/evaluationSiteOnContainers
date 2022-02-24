using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_company",
                table => new
                {
                    company_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    company_name = table.Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "游戏类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table => { table.PrimaryKey("PK_game_company", x => x.company_id); },
                comment: "发行公司")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_owner",
                table => new
                {
                    user_id = table.Column<string>("varchar(255)", nullable: false, comment: "用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    game_id = table.Column<int>("int", nullable: false, comment: "游戏信息id")
                },
                constraints: table => { table.PrimaryKey("PK_game_owner", x => new {x.user_id, x.game_id}); },
                comment: "玩家游戏拥有表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_tag",
                table => new
                {
                    tag_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tag_name = table.Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "游戏标签名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table => { table.PrimaryKey("PK_game_tag", x => x.tag_id); },
                comment: "游戏标签表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_type",
                table => new
                {
                    type_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_name = table
                        .Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "游戏类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>("tinyint(1)", nullable: true, comment: "逻辑删除")
                },
                constraints: table => { table.PrimaryKey("PK_game_type", x => x.type_id); },
                comment: "游戏类型分类表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_info",
                table => new
                {
                    game_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "游戏名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>("varchar(500)", maxLength: 500, nullable: false, comment: "游戏描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    details_picture = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "游戏展示图大图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rough_picture = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "游戏展示图小图")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    average_score = table.Column<double>("double", nullable: true, comment: "游戏评分"),
                    sell_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "发售时间"),
                    support_platform = table
                        .Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "游玩平台")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hot_points = table.Column<long>("bigint", nullable: true, comment: "游戏热度"),
                    company_id = table.Column<int>("int", nullable: true, comment: "游戏公司外键"),
                    category_id = table.Column<int>("int", nullable: true, comment: "游戏类别外键"),
                    game_playsuggestion_id = table.Column<int>("int", nullable: true, comment: "游戏游玩建议外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_info", x => x.game_id);
                    table.ForeignKey(
                        "FK_game_info_game_company_company_id",
                        x => x.company_id,
                        "game_company",
                        "company_id");
                    table.ForeignKey(
                        "FK_game_info_game_type_category_id",
                        x => x.category_id,
                        "game_type",
                        "type_id");
                },
                comment: "游戏信息表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "game_playsuggestion",
                table => new
                {
                    suggestion_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    operation_system = table
                        .Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "操作系统建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpu_name = table.Column<string>("varchar(50)", maxLength: 50, nullable: false, comment: "CPU型号建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    memory_size = table.Column<double>("double", nullable: false, comment: "内存大小建议"),
                    disk_size = table.Column<double>("double", nullable: false, comment: "磁盘大小建议"),
                    graphics_card = table.Column<string>("longtext", nullable: false, comment: "显卡型号建议")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    game_id = table.Column<int>("int", nullable: false, comment: "游戏外键id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_playsuggestion", x => x.suggestion_id);
                    table.ForeignKey(
                        "FK_game_playsuggestion_game_info_game_id",
                        x => x.game_id,
                        "game_info",
                        "game_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "游玩游戏配置建议表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "gameinfo_tag",
                table => new
                {
                    game_id = table.Column<int>("int", nullable: false, comment: "游戏id"),
                    tag_id = table.Column<int>("int", nullable: false, comment: "标签id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameinfo_tag", x => new {x.game_id, x.tag_id});
                    table.ForeignKey(
                        "FK_gameinfo_tag_game_info_game_id",
                        x => x.game_id,
                        "game_info",
                        "game_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_gameinfo_tag_game_tag_tag_id",
                        x => x.tag_id,
                        "game_tag",
                        "tag_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "游戏与标签的多对多链接表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "gameshop_items",
                table => new
                {
                    item_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    price = table.Column<decimal>("decimal(65,30)", nullable: false, comment: "售价"),
                    discount = table.Column<float>("float", nullable: false, comment: "折扣"),
                    available_stock = table.Column<int>("int", nullable: false, comment: "库存量"),
                    hotsell_point =
                        table.Column<double>("double", nullable: false, defaultValue: 10.0, comment: "销售热度"),
                    temporary_stopsell = table.Column<bool>("tinyint(1)", nullable: true, comment: "暂停销售"),
                    game_id = table.Column<int>("int", nullable: false, comment: "游戏信息外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameshop_items", x => x.item_id);
                    table.ForeignKey(
                        "FK_gameshop_items_game_info_game_id",
                        x => x.game_id,
                        "game_info",
                        "game_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "游戏售卖商品表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "gameitem_sdk",
                table => new
                {
                    sdk_id = table.Column<long>("bigint", nullable: false, comment: "SDK主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sdk_string = table
                        .Column<string>("varchar(450)", maxLength: 450, nullable: false, comment: "游戏SDK码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    has_send = table.Column<bool>("tinyint(1)", nullable: true, comment: "是否已卖出"),
                    sent_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "发送时间"),
                    item_id = table.Column<int>("int", nullable: false, comment: "游戏商品外键"),
                    row_version = table
                        .Column<DateTime>("datetime(6)", rowVersion: true, nullable: false, comment: "行版本")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gameitem_sdk", x => x.sdk_id);
                    table.ForeignKey(
                        "FK_gameitem_sdk_gameshop_items_item_id",
                        x => x.item_id,
                        "gameshop_items",
                        "item_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "sdk存放表, 负责给玩家发送未发送的sdk")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "gamesdk_player",
                table => new
                {
                    sdk_player_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>("varchar(255)", nullable: false, comment: "购买用户的Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    has_checked = table.Column<bool>("tinyint(1)", nullable: true, comment: "是否已经被校验"),
                    check_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "校验的时间"),
                    sdk_id = table.Column<long>("bigint", nullable: false, comment: "游戏发放的sdk外键")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamesdk_player", x => x.sdk_player_id);
                    table.ForeignKey(
                        "FK_gamesdk_player_gameitem_sdk_sdk_id",
                        x => x.sdk_id,
                        "gameitem_sdk",
                        "sdk_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "保存用户购买的SDK信息")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            "IX_game_info_category_id",
            "game_info",
            "category_id");

        migrationBuilder.CreateIndex(
            "IX_game_info_company_id",
            "game_info",
            "company_id");

        migrationBuilder.CreateIndex(
            "IX_game_playsuggestion_game_id",
            "game_playsuggestion",
            "game_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_gameinfo_tag_tag_id",
            "gameinfo_tag",
            "tag_id");

        migrationBuilder.CreateIndex(
            "IX_gameitem_sdk_item_id",
            "gameitem_sdk",
            "item_id");

        migrationBuilder.CreateIndex(
            "IX_gamesdk_player_sdk_id",
            "gamesdk_player",
            "sdk_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_gamesdk_player_user_id",
            "gamesdk_player",
            "user_id");

        migrationBuilder.CreateIndex(
            "IX_gameshop_items_game_id",
            "gameshop_items",
            "game_id",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "game_owner");

        migrationBuilder.DropTable(
            "game_playsuggestion");

        migrationBuilder.DropTable(
            "gameinfo_tag");

        migrationBuilder.DropTable(
            "gamesdk_player");

        migrationBuilder.DropTable(
            "game_tag");

        migrationBuilder.DropTable(
            "gameitem_sdk");

        migrationBuilder.DropTable(
            "gameshop_items");

        migrationBuilder.DropTable(
            "game_info");

        migrationBuilder.DropTable(
            "game_company");

        migrationBuilder.DropTable(
            "game_type");
    }
}