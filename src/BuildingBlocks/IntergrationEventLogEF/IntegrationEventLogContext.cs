namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;

public class IntegrationEventLogContext : DbContext
{
    public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) : base(options) { }

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
    }

    void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
    {
        builder.ToTable("IntegrationEventLog")
            .HasComment("事件日志表");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired()
            .HasComment("事件Id");

        builder.Property(e => e.Content)
            .IsRequired()
            .HasComment("事件内容");

        builder.Property(e => e.CreationTime)
            .IsRequired()
            .HasComment("记录时间");

        builder.Property(e => e.State)
            .IsRequired()
            .HasComment("事件状态");

        builder.Property(e => e.TimesSent)
            .IsRequired()
            .HasComment("发送次数");

        builder.Property(e => e.EventTypeName)
            .IsRequired()
            .HasComment("事件类型名");
    }
}
