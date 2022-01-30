namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

public class GameOwnerEntityTypeConfiguration : IEntityTypeConfiguration<GameOwner>
{
    public void Configure(EntityTypeBuilder<GameOwner> builder)
    {

        builder.HasKey(g => new { g.UserId, g.GameId });

        builder
            .HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}