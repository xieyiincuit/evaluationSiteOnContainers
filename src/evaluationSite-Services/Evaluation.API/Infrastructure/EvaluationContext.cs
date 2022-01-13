namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure;

public class EvaluationContext : DbContext
{
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
}

public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<EvaluationContext>
{
    public EvaluationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EvaluationContext>()
            .UseSqlServer("Server=.;Initial Catalog=evaluationSiteOnContainers.Services.evaluation;Integrated Security=true"); ;

        return new EvaluationContext(optionsBuilder.Options);
    }
}