using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackManage.API.Infrastructure.BackManageMigrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "approve_record",
                table => new
                {
                    approve_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>("varchar(200)", maxLength: 200, nullable: false, comment: "申请用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approve_body = table.Column<string>("varchar(1000)", maxLength: 1000, nullable: false,
                            comment: "测评审批信息正文")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apply_time = table.Column<DateTime>("datetime(6)", nullable: false,
                        defaultValue: new DateTime(2022, 2, 21, 13, 28, 19, 564, DateTimeKind.Local).AddTicks(1728),
                        comment: "申请时间"),
                    approve_status = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "审批状态"),
                    approve_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "审批时间"),
                    approve_user = table.Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "审批人")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table => { table.PrimaryKey("PK_approve_record", x => x.approve_id); },
                comment: "测评资格申请表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "banned_record",
                table => new
                {
                    banned_id = table.Column<int>("int", nullable: false, comment: "主键")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    banned_user_id = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: false, comment: "被举报用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    report_count = table.Column<int>("int", nullable: false, defaultValue: 1, comment: "被举报次数"),
                    banned_time = table.Column<DateTime>("datetime(6)", nullable: true, comment: "冻结时间"),
                    approve_user = table.Column<string>("varchar(200)", maxLength: 200, nullable: true, comment: "审批人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    banned_status = table.Column<int>("int", nullable: false, defaultValue: 0, comment: "封禁状态")
                },
                constraints: table => { table.PrimaryKey("PK_banned_record", x => x.banned_id); },
                comment: "用户举报记录表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "banned_user_link",
                table => new
                {
                    banned_user_id = table
                        .Column<string>("varchar(200)", maxLength: 200, nullable: false, comment: "被举报用户id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    check_user_id = table.Column<string>("varchar(200)", maxLength: 200, nullable: false,
                            comment: "发起举报用户id")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banned_user_link", x => new { x.banned_user_id, x.check_user_id });
                },
                comment: "用户举报链接表")
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            "IX_approve_record_user_id",
            "approve_record",
            "user_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_banned_record_banned_user_id",
            "banned_record",
            "banned_user_id",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "approve_record");

        migrationBuilder.DropTable(
            "banned_record");

        migrationBuilder.DropTable(
            "banned_user_link");
    }
}