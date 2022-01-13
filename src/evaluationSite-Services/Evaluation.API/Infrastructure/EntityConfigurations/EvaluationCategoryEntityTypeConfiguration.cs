namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;

public class EvaluationCategoryEntityTypeConfiguration : IEntityTypeConfiguration<EvaluationCategory>
{
    public void Configure(EntityTypeBuilder<EvaluationCategory> builder)
    {
        builder.ToTable("evaluation_category")
            .HasComment("测评文章分类表");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasComment("测评类别主键")
            .IsRequired();

        builder.Property(cb => cb.Type)
            .HasColumnName("type")
            .HasComment("测评类别名")
            .IsRequired()
            .HasMaxLength(10);
    }
}

