﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameRepoContext : DbContext
{
    public GameRepoContext(DbContextOptions<GameRepoContext> options) : base(options) { }

    public DbSet<GameInfo> GameInfos { get; set; }
    public DbSet<GamePlaySuggestion> PlaySuggestions { get; set; }
    public DbSet<GameTag> GameTags { get; set; }
    public DbSet<GameCategory> GameCategories { get; set; }
    public DbSet<GameCompany> GameCompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //多对多创建关系表
        modelBuilder.Entity<GameInfo>()
             .HasMany(g => g.GameTags)
             .WithMany(t => t.GameInfos)
             .UsingEntity<GameInfoTag>(
                j => j
                .HasOne(t => t.GameTag)
                .WithMany(g => g.GameInfoTags)
                .HasForeignKey(f => f.TagId),
                j => j
                .HasOne(t => t.GameInfo)
                .WithMany(g => g.GameInfoTags)
                .HasForeignKey(f => f.GameId),
                j => j.HasKey(t => new { t.GameId, t.TagId })
                );

        modelBuilder.Entity<GameInfo>()
            .HasOne(g => g.GamePlaySuggestion)
            .WithOne(g => g.GameInfo)
            .HasForeignKey<GamePlaySuggestion>(pl => pl.GameId)
            .OnDelete(DeleteBehavior.Cascade);
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
