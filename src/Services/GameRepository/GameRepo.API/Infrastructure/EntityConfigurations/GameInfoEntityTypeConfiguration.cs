namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameInfoEntityTypeConfiguration : IEntityTypeConfiguration<GameInfo>
{
    public void Configure(EntityTypeBuilder<GameInfo> builder)
    {
        //多对多创建关系表
        builder
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

        builder
            .HasOne(g => g.GamePlaySuggestion)
            .WithOne(g => g.GameInfo)
            .HasForeignKey<GamePlaySuggestion>(pl => pl.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(g => g.GameShopItem)
            .WithOne(g => g.GameInfo)
            .HasForeignKey<GameShopItem>(pl => pl.GameInfoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}