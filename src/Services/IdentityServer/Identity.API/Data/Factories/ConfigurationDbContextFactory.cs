namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data.Factories;

public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
           .AddJsonFile("appsettings.json")
           .AddEnvironmentVariables()
           .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        var storeOptions = new ConfigurationStoreOptions();
        var connectionString = config.GetConnectionString("IdentityConnection");
        optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction: o => o.MigrationsAssembly("Identity.API"));

        return new ConfigurationDbContext(optionsBuilder.Options, storeOptions);
    }
}
