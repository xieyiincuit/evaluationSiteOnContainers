using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    public partial class AddhotpointandComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_company_GameCompanyId",
                table: "game_info");

            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_type_GameCategoryId",
                table: "game_info");

            migrationBuilder.DropForeignKey(
                name: "FK_gameinfo_tag_game_info_GameId",
                table: "gameinfo_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_gameinfo_tag_game_tag_TagId",
                table: "gameinfo_tag");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "gameinfo_tag",
                newName: "tag_id");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "gameinfo_tag",
                newName: "game_id");

            migrationBuilder.RenameIndex(
                name: "IX_gameinfo_tag_TagId",
                table: "gameinfo_tag",
                newName: "IX_gameinfo_tag_tag_id");

            migrationBuilder.RenameColumn(
                name: "GameCompanyId",
                table: "game_info",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "GameCategoryId",
                table: "game_info",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_info_GameCompanyId",
                table: "game_info",
                newName: "IX_game_info_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_info_GameCategoryId",
                table: "game_info",
                newName: "IX_game_info_category_id");

            migrationBuilder.AlterColumn<double>(
                name: "memory_size",
                table: "play_suggestion",
                type: "double",
                nullable: false,
                comment: "内存大小建议",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "内存大小建议");

            migrationBuilder.AlterColumn<double>(
                name: "disk_size",
                table: "play_suggestion",
                type: "double",
                nullable: false,
                comment: "磁盘大小建议",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "磁盘大小建议");

            migrationBuilder.AlterColumn<int>(
                name: "tag_id",
                table: "gameinfo_tag",
                type: "int",
                nullable: false,
                comment: "标签id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "game_id",
                table: "gameinfo_tag",
                type: "int",
                nullable: false,
                comment: "游戏id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "category_name",
                table: "game_type",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "游戏类型名",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldComment: "游戏类型名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tag_name",
                table: "game_tag",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "游戏标签名",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldComment: "游戏标签名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "routh_picture",
                table: "game_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "游戏展示图小图",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "游戏展示图小图")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "details_picture",
                table: "game_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "游戏展示图大图",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "游戏展示图大图")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "game_info",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "游戏描述",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldComment: "游戏描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "hot_points",
                table: "game_info",
                type: "bigint",
                nullable: true,
                comment: "游戏热度");

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                table: "game_company",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "游戏类型名",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10,
                oldComment: "游戏类型名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info",
                column: "company_id",
                principalTable: "game_company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info",
                column: "category_id",
                principalTable: "game_type",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameinfo_tag_game_info_game_id",
                table: "gameinfo_tag",
                column: "game_id",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameinfo_tag_game_tag_tag_id",
                table: "gameinfo_tag",
                column: "tag_id",
                principalTable: "game_tag",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_company_company_id",
                table: "game_info");

            migrationBuilder.DropForeignKey(
                name: "FK_game_info_game_type_category_id",
                table: "game_info");

            migrationBuilder.DropForeignKey(
                name: "FK_gameinfo_tag_game_info_game_id",
                table: "gameinfo_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_gameinfo_tag_game_tag_tag_id",
                table: "gameinfo_tag");

            migrationBuilder.DropColumn(
                name: "hot_points",
                table: "game_info");

            migrationBuilder.RenameColumn(
                name: "tag_id",
                table: "gameinfo_tag",
                newName: "TagId");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "gameinfo_tag",
                newName: "GameId");

            migrationBuilder.RenameIndex(
                name: "IX_gameinfo_tag_tag_id",
                table: "gameinfo_tag",
                newName: "IX_gameinfo_tag_TagId");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "game_info",
                newName: "GameCompanyId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "game_info",
                newName: "GameCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_game_info_company_id",
                table: "game_info",
                newName: "IX_game_info_GameCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_game_info_category_id",
                table: "game_info",
                newName: "IX_game_info_GameCategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "memory_size",
                table: "play_suggestion",
                type: "int",
                nullable: false,
                comment: "内存大小建议",
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "内存大小建议");

            migrationBuilder.AlterColumn<int>(
                name: "disk_size",
                table: "play_suggestion",
                type: "int",
                nullable: false,
                comment: "磁盘大小建议",
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "磁盘大小建议");

            migrationBuilder.AlterColumn<int>(
                name: "TagId",
                table: "gameinfo_tag",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "标签id");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "gameinfo_tag",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "游戏id");

            migrationBuilder.AlterColumn<string>(
                name: "category_name",
                table: "game_type",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "游戏类型名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "游戏类型名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tag_name",
                table: "game_tag",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "游戏标签名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "游戏标签名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "routh_picture",
                table: "game_info",
                type: "longtext",
                nullable: true,
                comment: "游戏展示图小图",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "游戏展示图小图")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "details_picture",
                table: "game_info",
                type: "longtext",
                nullable: true,
                comment: "游戏展示图大图",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "游戏展示图大图")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "game_info",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "游戏描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "游戏描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                table: "game_company",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "游戏类型名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "游戏类型名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_company_GameCompanyId",
                table: "game_info",
                column: "GameCompanyId",
                principalTable: "game_company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_info_game_type_GameCategoryId",
                table: "game_info",
                column: "GameCategoryId",
                principalTable: "game_type",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameinfo_tag_game_info_GameId",
                table: "gameinfo_tag",
                column: "GameId",
                principalTable: "game_info",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gameinfo_tag_game_tag_TagId",
                table: "gameinfo_tag",
                column: "TagId",
                principalTable: "game_tag",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
