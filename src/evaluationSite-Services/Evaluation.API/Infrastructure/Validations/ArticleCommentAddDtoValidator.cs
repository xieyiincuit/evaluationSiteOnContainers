namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class ArticleCommentAddDtoValidator : AbstractValidator<ArticleCommentAddDto>
{
    public ArticleCommentAddDtoValidator()
    {
        RuleFor(r => r.ArticleId)
            .NotNull().WithMessage("required | 未能识别到评测文章")
            .GreaterThan(0).WithMessage("invalid | 非法参数: articleId")
            .LessThan(int.MaxValue).WithMessage("invalid | 非法参数: articleId");

        RuleFor(a => a.Content)
            .NotEmpty().WithMessage("required | 请输入您的评论内容")
            .MaximumLength(500).WithMessage("length | 评论内容控制在500字以内");
    }
}