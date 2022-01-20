namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

public class EvaluationCategoryEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationCategory>
{
    public void Configure(EntityTypeBuilder<EvaluationCategory> builder)
    {
        builder.ToTable("evaluation_category")
            .HasComment("测评文章分类表");

        builder.HasKey(ci => ci.CategoryId);

        builder.Property(ci => ci.CategoryId)
            .HasColumnName("category_id")
            .ValueGeneratedOnAdd()
            .HasComment("测评类别主键")
            .IsRequired();

        builder.Property(cb => cb.CategoryType)
            .HasColumnName("category_type")
            .HasComment("测评类别名")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(cb => cb.IsDeleted)
            .HasColumnName("is_deleted")
            .HasComment("逻辑删除")
            .IsRequired(false)
            .HasDefaultValue(false);
    }
}