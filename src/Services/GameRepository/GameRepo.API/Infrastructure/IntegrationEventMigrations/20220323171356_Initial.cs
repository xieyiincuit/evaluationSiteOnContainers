using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationEventLogEF.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IntegrationEventLogs",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "事件Id", collation: "ascii_general_ci"),
                    EventTypeName = table.Column<string>(type: "longtext", nullable: false, comment: "事件类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<int>(type: "int", nullable: false, comment: "事件状态: 2-发送执行成功 3-发送但执行失败"),
                    TimesSent = table.Column<int>(type: "int", nullable: false, comment: "发送次数"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "记录时间"),
                    Content = table.Column<string>(type: "longtext", nullable: false, comment: "事件内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionId = table.Column<string>(type: "longtext", nullable: false, comment: "事务Id")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogs", x => x.EventId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLogs");
        }
    }
}
