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
                name: "integrationevent_log",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "事件Id", collation: "ascii_general_ci"),
                    event_type_name = table.Column<string>(type: "longtext", nullable: false, comment: "事件类型名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<int>(type: "int", nullable: false, comment: "事件状态"),
                    times_sent = table.Column<int>(type: "int", nullable: false, comment: "发送次数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "记录时间"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "事件内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    transaction_id = table.Column<string>(type: "longtext", nullable: false, comment: "事务Id")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integrationevent_log", x => x.event_id);
                },
                comment: "事件日志表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integrationevent_log");
        }
    }
}
