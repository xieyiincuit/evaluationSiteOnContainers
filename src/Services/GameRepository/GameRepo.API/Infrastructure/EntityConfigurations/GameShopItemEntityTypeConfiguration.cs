namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameShopItemEntityTypeConfiguration : IEntityTypeConfiguration<GameShopItem>
{
    public void Configure(EntityTypeBuilder<GameShopItem> builder)
    {
        //多对多创建关系表
        builder.Property(x => x.HotSellPoint)
            .HasDefaultValue(10);

        builder
            .HasOne<GameInfo>()
            .WithOne()
            .HasForeignKey<GameShopItem>(x => x.GameInfoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}