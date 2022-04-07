using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackManage.API.Infrastructure.BackManageMigrations
{
    public partial class ChangeBodyLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "approve_body",
                table: "approve_record",
                type: "longtext",
                nullable: false,
                comment: "测评审批信息正文",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldComment: "测评审批信息正文")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "apply_time",
                table: "approve_record",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 14, 9, 50, 980, DateTimeKind.Local).AddTicks(8253),
                comment: "申请时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2022, 2, 21, 13, 28, 19, 564, DateTimeKind.Local).AddTicks(1728),
                oldComment: "申请时间");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "approve_body",
                table: "approve_record",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "测评审批信息正文",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "测评审批信息正文")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "apply_time",
                table: "approve_record",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2022, 2, 21, 13, 28, 19, 564, DateTimeKind.Local).AddTicks(1728),
                comment: "申请时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2022, 4, 7, 14, 9, 50, 980, DateTimeKind.Local).AddTicks(8253),
                oldComment: "申请时间");
        }
    }
}
