using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationEventLogEF.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "IntegrationEventLog",
            table => new
            {
                EventId = table.Column<Guid>("uniqueidentifier", nullable: false, comment: "事件Id"),
                EventTypeName = table.Column<string>("nvarchar(max)", nullable: false, comment: "事件类型名"),
                State = table.Column<int>("int", nullable: false, comment: "事件状态: 2-发送执行成功 3-发送但执行失败"),
                TimesSent = table.Column<int>("int", nullable: false, comment: "发送次数"),
                CreateTime = table.Column<DateTime>("datetime2", nullable: false, comment: "记录时间"),
                Content = table.Column<string>("nvarchar(max)", nullable: false, comment: "事件内容"),
                TransactionId = table.Column<string>("nvarchar(max)", nullable: false, comment: "事务Id")
            },
            constraints: table => { table.PrimaryKey("PK_integrationevent_log", x => x.EventId); },
            comment: "事件日志表");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "IntegrationEventLog");
    }
}