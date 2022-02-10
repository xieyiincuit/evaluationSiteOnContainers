namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameShopItemEntityTypeConfiguration : IEntityTypeConfiguration<GameShopItem>
{
    public void Configure(EntityTypeBuilder<GameShopItem> builder)
    {
        builder.Property(x => x.HotSellPoint)
            .HasDefaultValue(10);

        builder
            .HasMany(g => g.GameSDKList)
            .WithOne(b => b.GameShopItem)
            .HasForeignKey(x => x.GameItemId)
            .OnDelete(DeleteBehavior.Restrict);

        //在DriveType GameInfo中定义Navigation
        //builder
        //    .HasOne<GameInfo>()
        //    .WithOne(x => x.GameShopItem)
        //    .HasForeignKey<GameShopItem>(x => x.GameInfoId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}