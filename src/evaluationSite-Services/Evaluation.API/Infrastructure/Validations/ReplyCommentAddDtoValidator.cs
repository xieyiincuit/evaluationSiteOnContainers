namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class ReplyCommentAddDtoValidator : AbstractValidator<ReplyCommentAddDto>
{
    public ReplyCommentAddDtoValidator()
    {
        RuleFor(r => r.ArticleId)
            .NotNull().WithMessage("required | 未能识别到评测文章")
            .GreaterThan(0).WithMessage("invalid | 非法参数: articleId");

        RuleFor(r => r.Content)
            .NotEmpty().WithMessage("required | 请输入您的评论内容")
            .MaximumLength(500).WithMessage("length | 评论内容控制在500字以内");

        RuleFor(r => r.ReplyUserId)
            .NotNull().WithMessage("required | 回复的用户id不应为空");
    }
}
