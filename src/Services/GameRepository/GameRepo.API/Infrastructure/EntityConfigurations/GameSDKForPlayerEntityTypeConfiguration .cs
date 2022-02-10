namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameSDKForPlayerEntityTypeConfiguration : IEntityTypeConfiguration<GameSDKForPlayer>
{
    public void Configure(EntityTypeBuilder<GameSDKForPlayer> builder)
    {
        builder.HasIndex(x => x.UserId);
    }
}