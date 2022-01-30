using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class AddGameShop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_owner",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    game_id = table.Column<int>(type: "int", nullable: false, comment: "游戏商品id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_owner", x => new { x.user_id, x.game_id });
                    table.ForeignKey(
                        name: "FK_game_owner_game_info_game_id",
                        column: x => x.game_id,
                        principalTable: "game_info",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "玩家游戏拥有表")
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
                    item_id = table.Column<int>(type: "int", nullable: false, comment: "游戏商品外键")
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
                    user_id = table.Column<string>(type: "longtext", nullable: false, comment: "购买用户的Id")
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
                name: "IX_game_owner_game_id",
                table: "game_owner",
                column: "game_id");

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
                name: "gamesdk_player");

            migrationBuilder.DropTable(
                name: "gameitem_sdk");

            migrationBuilder.DropTable(
                name: "gameshop_items");
        }
    }
}
