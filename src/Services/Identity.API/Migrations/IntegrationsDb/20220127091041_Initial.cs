using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationEventLogEF.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "integrationevent_log",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "事件Id"),
                    event_type_name = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事件类型名"),
                    state = table.Column<int>(type: "int", nullable: false, comment: "事件状态: 2-发送执行成功 3-发送但执行失败"),
                    times_sent = table.Column<int>(type: "int", nullable: false, comment: "发送次数"),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "记录时间"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事件内容"),
                    transaction_id = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事务Id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integrationevent_log", x => x.event_id);
                },
                comment: "事件日志表");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integrationevent_log");
        }
    }
}
