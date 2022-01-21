namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.IntegrationEventMigrations;

public class IntegrationEventLogContextDesignTimeFactory
{
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

        optionsBuilder.UseMySql(".", new MySqlServerVersion(new Version(8, 0, 27)),
            options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }
}