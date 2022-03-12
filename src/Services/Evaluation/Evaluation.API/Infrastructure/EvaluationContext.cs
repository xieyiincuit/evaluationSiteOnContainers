namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContext : DbContext
{
    public EvaluationContext(DbContextOptions<EvaluationContext> options) : base(options)
    {
    }

    public DbSet<EvaluationArticle> Articles { get; set; }
    public DbSet<EvaluationCategory> Categories { get; set; }
    public DbSet<EvaluationComment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EvaluationArticleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationCategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationCommentEntityTypeConfiguration());
    }
}

public class EvaluationContextDesignFactory : IDesignTimeDbContextFactory<EvaluationContext>
{
    public EvaluationContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<EvaluationContext>();
        var connectionString = configuration.GetConnectionString("EvaluationDbConnectString");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        builder.UseMySql(connectionString, serverVersion);

        return new EvaluationContext(builder.Options);
    }
    //.NET Cli
    // dotnet ef migrations add Initial -o ./Infrastructure/EvaluationMigrations -v
}