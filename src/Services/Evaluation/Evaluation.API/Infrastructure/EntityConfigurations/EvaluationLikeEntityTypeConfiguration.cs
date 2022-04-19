namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

public class EvaluationLikeEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationLikeRecord>
{
    public void Configure(EntityTypeBuilder<EvaluationLikeRecord> builder)
    {
        builder.ToTable("evaluation_like")
            .HasComment("测评文章点赞记录表");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.ArticleId)
            .HasColumnName("article_id")
            .HasComment("文章id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasComment("用户id");

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time")
            .HasComment("点赞时间")
            .HasDefaultValue(DateTime.Now.ToLocalTime());

        builder.HasOne(x => x.EvaluationArticle)
            .WithMany(x => x.EvaluationLikeRecords)
            .HasForeignKey(x => x.ArticleId)
            .HasConstraintName("foreignKey_likeRecord_article")
            .OnDelete(DeleteBehavior.Cascade);
    }
}