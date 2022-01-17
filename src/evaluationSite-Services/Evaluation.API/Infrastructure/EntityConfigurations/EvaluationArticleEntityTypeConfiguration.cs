namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

class EvaluationArticleEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationArticle>
{
    public void Configure(EntityTypeBuilder<EvaluationArticle> builder)
    {
        builder.ToTable("evaluation_article")
            .HasComment("游戏测评文章信息表");

        builder.HasKey(x => x.ArticleId);

        builder.Property(x => x.ArticleId)
            .HasColumnName("article_id")
            .HasComment("主键")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasComment("测评内容作者id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasComment("测评文章标题")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ArticleImage)
          .HasColumnName("article_image")
          .HasComment("内容Top呈现图")
          .IsRequired(false);

        builder.Property(x => x.DesciptionImage)
         .HasColumnName("desciption_image")
         .HasComment("展示略缩图")
         .IsRequired(false);

        builder.Property(x => x.Content)
            .HasColumnName("content")
            .HasComment("测评文章内容")
            .IsRequired();

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time")
            .HasComment("测评内容创建时间")
            .IsRequired();

        builder.Property(x => x.UpdateTime)
            .HasColumnName("update_time")
            .HasComment("测评内容更新时间")
            .IsRequired(false);

        builder.Property(x => x.JoinCount)
            .HasColumnName("join_count")
            .HasComment("文章浏览量")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.CommentsCount)
            .HasColumnName("comments_count")
            .HasComment("文章评论数量")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.SupportCount)
            .HasColumnName("support_count")
            .HasComment("文章点赞数量")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasComment("逻辑删除")
            .IsRequired(false);

        builder.Property(x => x.Description)
           .HasColumnName("description")
           .HasComment("文章测评简介")
           .IsRequired(false)
           .HasMaxLength(100);

        builder.Property(x => x.GameId)
            .HasColumnName("game_id")
            .HasComment("测评内容关联游戏id")
            .IsRequired();

        builder.Property(x => x.GameName)
            .HasColumnName("game_name")
            .HasComment("测评内容关联游戏名")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("article_status")
            .HasComment("文章发布状态")
            .IsRequired();

        builder.Property(x => x.CategoryTypeId)
            .HasColumnName("category_type_id")
            .HasComment("测评类别主键")
            .IsRequired(false);

        builder.HasOne(x => x.CategoryType)
            .WithMany()
            .HasForeignKey(x => x.CategoryTypeId)
            .HasConstraintName("foreignKey_type_article");
    }
}