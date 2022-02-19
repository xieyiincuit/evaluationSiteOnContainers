namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

public class BackManageContext : DbContext
{
    public BackManageContext(DbContextOptions<BackManageContext> options) : base(options) { }

}


public class BackManageContextDesignFactory : IDesignTimeDbContextFactory<BackManageContext>
{
    public BackManageContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<BackManageContext>();
        var connectionString = configuration["BackDbConnectString"];
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        builder.UseMySql(connectionString, serverVersion);
        return new BackManageContext(builder.Options);
    }

    //.NET ClI
    //dotnet ef migrations add Initial -o ./Infrastructure/BackManageMigrations -c BackManageContext -v
}