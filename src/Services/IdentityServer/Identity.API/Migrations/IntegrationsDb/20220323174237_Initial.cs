using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationEventLogEF.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEventLogs",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "事件Id"),
                    EventTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事件类型名"),
                    State = table.Column<int>(type: "int", nullable: false, comment: "事件状态: 2-发送执行成功 3-发送但执行失败"),
                    TimesSent = table.Column<int>(type: "int", nullable: false, comment: "发送次数"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "记录时间"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事件内容"),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "事务Id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLogs", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLogs");
        }
    }
}
