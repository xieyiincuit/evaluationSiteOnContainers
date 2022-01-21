namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class CategoryAddDtoValidator : AbstractValidator<CategoryAddDto>
{
    public CategoryAddDtoValidator()
    {
        RuleFor(c => c.CategoryType)
            .NotEmpty().WithMessage("required | 请输入测评类别")
            .MaximumLength(10).WithMessage("length | 测评类别不应超过10个字符");
    }
}