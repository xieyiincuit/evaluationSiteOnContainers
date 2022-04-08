namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

public class BackManageContext : DbContext
{
    public BackManageContext(DbContextOptions<BackManageContext> options) : base(options)
    {
    }

    public DbSet<ApproveRecord> ApproveRecords { get; set; }
    public DbSet<BannedRecord> BannedRecords { get; set; }
    public DbSet<BannedUserLink> BannedUserLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApproveRecord>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<ApproveRecord>()
            .Property(x => x.ApplyTime)
            .HasDefaultValue(DateTime.Now.ToLocalTime());

        modelBuilder.Entity<ApproveRecord>()
            .Property(x => x.Status)
            .HasDefaultValue(ApproveStatus.Progressing);

        modelBuilder.Entity<BannedRecord>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<BannedRecord>()
            .Property(x => x.ReportCount)
            .HasDefaultValue(1);

        modelBuilder.Entity<BannedRecord>()
            .Property(x => x.Status)
            .HasDefaultValue(BannedStatus.Checking);

        modelBuilder.Entity<BannedUserLink>()
            .HasKey(x => new { x.BannedUserId, x.CheckUserId });
    }
}

public class BackManageContextDesignFactory : IDesignTimeDbContextFactory<BackManageContext>
{
    public BackManageContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<BackManageContext>();
        var connectionString = configuration["BackDbConnectString"];
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        builder.UseMySql(connectionString, serverVersion);
        return new BackManageContext(builder.Options);
    }

    //EFCore CLI
    //dotnet ef migrations add Initial -o ./Infrastructure/BackManageMigrations -c BackManageContext -v
}