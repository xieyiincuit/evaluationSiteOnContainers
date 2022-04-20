namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class ArticleLikeAddDtoValidator : AbstractValidator<ArticleLikeAddDto>
{
    public ArticleLikeAddDtoValidator()
    {
        RuleFor(x => x.ArticleId)
            .NotNull().WithMessage("required | 请选择您点赞的测评文章")
            .GreaterThan(0).WithMessage("invalid | 不存在的文章")
            .LessThan(int.MaxValue).WithMessage("invalid | 不存在的文章");
    }
}