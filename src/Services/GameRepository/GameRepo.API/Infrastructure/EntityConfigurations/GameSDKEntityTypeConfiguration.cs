namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameSDKEntityTypeConfiguration : IEntityTypeConfiguration<GameItemSDK>
{
    public void Configure(EntityTypeBuilder<GameItemSDK> builder)
    {
        builder
            .HasOne(sdk => sdk.GameSDKForPlayer)
            .WithOne(x => x.GameItemSDK)
            .HasForeignKey<GameSDKForPlayer>(x => x.SDKItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}