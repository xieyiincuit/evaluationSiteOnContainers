namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class ReplyCommentAddDtoValidator : AbstractValidator<ReplyCommentAddDto>
{
    public ReplyCommentAddDtoValidator()
    {
        RuleFor(r => r.ArticleId)
            .GreaterThan(0).WithMessage("invalid | 非法参数: articleId")
            .LessThan(int.MaxValue).WithMessage("invalid | 非法参数: articleId");

        RuleFor(r => r.Content)
            .NotEmpty().WithMessage("required | 请输入您的评论内容")
            .MaximumLength(500).WithMessage("length | 评论内容控制在500字以内");

        RuleFor(r => r.ReplyCommentId)
            .GreaterThan(0).WithMessage("invalid | 非法参数: replyCommentId")
            .LessThan(int.MaxValue).WithMessage("invalid | 非法参数: replyCommentId");

        RuleFor(r => r.RootCommentId)
            .GreaterThan(0).WithMessage("invalid | 非法参数: rootCommentId")
            .LessThan(int.MaxValue).WithMessage("invalid | 非法参数: rootCommentId");
    }
}