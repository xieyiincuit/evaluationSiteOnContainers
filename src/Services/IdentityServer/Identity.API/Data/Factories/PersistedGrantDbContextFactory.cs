namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data.Factories;

public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
{
    public PersistedGrantDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
        var operationOptions = new OperationalStoreOptions();
        var connectionString = config.GetConnectionString("IdentityConnection");
        optionsBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly("Identity.API"));

        return new PersistedGrantDbContext(optionsBuilder.Options, operationOptions);
    }
}