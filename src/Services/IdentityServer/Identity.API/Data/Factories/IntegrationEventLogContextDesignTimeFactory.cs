namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data.Factories;

public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
{
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

        optionsBuilder.UseSqlServer(connectionString,
            options => options.MigrationsAssembly(typeof(IntegrationEventLogContext).GetTypeInfo().Assembly.GetName().Name));

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }

    //dotnet ef migrations add Initial -p ../../BuildingBlocks/IntegrationEventLogEF -o ../../Services/Identity.API/Migrations/IntegrationsDb -c IntegrationEventLogContext -v
}