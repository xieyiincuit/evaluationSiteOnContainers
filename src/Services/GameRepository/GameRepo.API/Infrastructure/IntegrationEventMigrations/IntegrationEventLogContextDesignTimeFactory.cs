namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.IntegrationEventMigrations;

public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
{
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("IntegrationDbConnectString");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

        optionsBuilder.UseMySql(connectionString, serverVersion,
            options => options.MigrationsAssembly(typeof(IntegrationEventLogContext).GetTypeInfo().Assembly.GetName().Name));

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }

    //dotnet ef migrations add Initial -p ../../BuildingBlocks/IntegrationEventLogEF -o ../../Services/GameRepo.API/Infrastructure/IntegrationEventMigrations -c IntegrationEventLogContext -v
}