namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data.Factories;

public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("IdentityConnection");
        builder.UseSqlServer(connectionString, sqlServerOptionsAction: o => o.MigrationsAssembly("Identity.API"));

        return new ApplicationDbContext(builder.Options);
    }
    //.NET Cli
    // dotnet ef migrations add UserInit -c ApplicationDbContext -o ./Migrations -v
}