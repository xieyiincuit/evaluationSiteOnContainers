namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        RuleFor(c => c.CategoryId)
            .GreaterThan(0).WithMessage("invalid | 非法参数: categoryId")
            .LessThan(int.MaxValue).WithMessage("invalid | 非法参数: categoryId");

        RuleFor(c => c.CategoryType)
            .NotEmpty().WithMessage("required | 请输入测评类别")
            .MaximumLength(10).WithMessage("length | 测评类别不应超过10个字符");
    }
}
