namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameSDKEntityTypeConfiguration : IEntityTypeConfiguration<GameItemSDK>
{
    public void Configure(EntityTypeBuilder<GameItemSDK> builder)
    {
        builder
            .HasOne<GameShopItem>()
            .WithMany(b => b.GameSDKList)
            .HasForeignKey(x => x.GameItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}