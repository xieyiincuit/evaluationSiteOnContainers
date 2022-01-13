namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

public class EvaluationCommentEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationComment>
{
    public void Configure(EntityTypeBuilder<EvaluationComment> builder)
    {
        builder.ToTable("evaluation_comment")
            .HasComment("测评文章评论表");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasComment("评论主键")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(e => e.Content)
            .HasColumnName("content")
            .HasComment("评论内容")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.LikeNums)
            .HasColumnName("like_nums")
            .HasComment("评论点赞数量")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.UserId)
           .HasColumnName("user_id")
           .HasComment("用户id")
           .IsRequired();

        builder.Property(e => e.UserName)
          .HasColumnName("user_name")
          .HasComment("用户名")
          .IsRequired();

        builder.Property(e => e.UserAvatar)
          .HasColumnName("user_avatar")
          .HasComment("用户头像")
          .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .HasComment("逻辑删除")
            .IsRequired(false);

        builder.Property(e => e.CreateTime)
            .HasColumnName("create_time")
            .HasComment("评论时间")
            .IsRequired();

        builder.Property(e => e.IsReplay)
            .HasColumnName("is_replay")
            .HasComment("该评论是否为回复")
            .IsRequired(false);

        builder.Property(e => e.ReplayId)
           .HasColumnName("replay_id")
           .HasComment("回复的评论id")
           .IsRequired(false);

        //设置外键约束
        builder.HasOne(e => e.EvaluationArticle)
            .WithMany(article => article.EvaluationComments)
            .HasForeignKey(e => e.ArticleForeignKey)
            .HasConstraintName("foreignKey_comment_article")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

