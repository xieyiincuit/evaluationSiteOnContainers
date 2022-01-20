namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

public class EvaluationCommentEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationComment>
{
    public void Configure(EntityTypeBuilder<EvaluationComment> builder)
    {
        builder.ToTable("evaluation_comment")
            .HasComment("测评文章评论表");

        builder.HasKey(e => e.CommentId);

        builder.Property(e => e.CommentId)
            .HasColumnName("comment_id")
            .HasComment("评论主键")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(e => e.Content)
            .HasColumnName("content")
            .HasComment("评论内容")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.SupportCount)
            .HasColumnName("support_count")
            .HasComment("评论点赞数量")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasComment("用户id")
            .IsRequired();

        builder.Property(e => e.NickName)
            .HasColumnName("nick_name")
            .HasComment("用户名")
            .IsRequired();

        builder.Property(e => e.Avatar)
            .HasColumnName("avatar")
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

        builder.Property(e => e.ReplayCommentId)
            .HasColumnName("replay_comment_id")
            .HasComment("回复的评论id")
            .IsRequired(false);

        builder.Property(e => e.ReplyUserId)
            .HasColumnName("replay_userid")
            .HasComment("回复的玩家Id")
            .IsRequired(false);

        builder.Property(e => e.ReplyNickName)
            .HasColumnName("replay_nickname")
            .HasComment("回复的玩家名")
            .IsRequired(false);

        builder.Property(e => e.RootCommentId)
            .HasColumnName("root_comment_id")
            .HasComment("回复评论属于哪个主评论")
            .IsRequired(false);

        builder.Property(e => e.ArticleId)
            .HasColumnName("article_id")
            .HasComment("评论对应的测评id")
            .IsRequired();

        //设置外键约束
        builder.HasOne(e => e.EvaluationArticle)
            .WithMany(article => article.EvaluationComments)
            .HasForeignKey(e => e.ArticleId)
            .HasConstraintName("foreignKey_comment_article")
            .OnDelete(DeleteBehavior.Cascade);
    }
}