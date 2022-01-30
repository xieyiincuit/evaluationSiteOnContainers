namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameSDKForPlayerEntityTypeConfiguration : IEntityTypeConfiguration<GameSDKForPlayer>
{
    public void Configure(EntityTypeBuilder<GameSDKForPlayer> builder)
    {
        builder
            .HasOne<GameItemSDK>()
            .WithOne()
            .HasForeignKey<GameSDKForPlayer>(x => x.SDKItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}