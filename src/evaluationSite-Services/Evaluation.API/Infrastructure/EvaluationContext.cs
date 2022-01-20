namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContext : DbContext
{
    //Print EF Query SQL ON Console
    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public EvaluationContext(DbContextOptions<EvaluationContext> options) : base(options) { }

    public DbSet<EvaluationArticle> Articles { get; set; }
    public DbSet<EvaluationCategory> Categories { get; set; }
    public DbSet<EvaluationComment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EvaluationArticleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationCategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationCommentEntityTypeConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //开启以下EF日志辅助开发
        optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }
}

public class EvaluationContextDesignFactory : IDesignTimeDbContextFactory<EvaluationContext>
{
    public EvaluationContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        var builder = new DbContextOptionsBuilder<EvaluationContext>();
        var connectionString = configuration.GetConnectionString("DataBaseConnectString");
        builder.UseSqlServer(connectionString); ;

        return new EvaluationContext(builder.Options);
    }
}