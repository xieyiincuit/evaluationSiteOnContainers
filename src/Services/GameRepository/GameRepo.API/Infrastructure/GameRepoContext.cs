namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameRepoContext : DbContext
{
    public GameRepoContext(DbContextOptions<GameRepoContext> options) : base(options) { }

    #region GameInfomation

    public DbSet<GameInfo> GameInfos { get; set; }
    public DbSet<GamePlaySuggestion> PlaySuggestions { get; set; }
    public DbSet<GameTag> GameTags { get; set; }
    public DbSet<GameCategory> GameCategories { get; set; }
    public DbSet<GameCompany> GameCompanies { get; set; }

    #endregion

    #region GameStore

    public DbSet<GameShopItem> GameShopItems { get; set; }
    public DbSet<GameItemSDK> GameItemSDKs { get; set; }
    public DbSet<GameSDKForPlayer> GameSDKForPlayers { get; set; }
    public DbSet<GameOwner> GameOwners { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameInfoEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GameShopItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GameSDKEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GameSDKForPlayerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GameOwnerEntityTypeConfiguration());
    }
}

public class GameRepoContextDesignFactory : IDesignTimeDbContextFactory<GameRepoContext>
{
    public GameRepoContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        var builder = new DbContextOptionsBuilder<GameRepoContext>();
        var connectionString = configuration.GetConnectionString("GameRepoDbConnectString");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
        builder.UseMySql(connectionString, serverVersion);
        return new GameRepoContext(builder.Options);
    }

    //.NET ClI
    //dotnet ef migrations add Initial -o ./Infrastructure/GameRepoMigrations -c GameRepoContext -v
}

