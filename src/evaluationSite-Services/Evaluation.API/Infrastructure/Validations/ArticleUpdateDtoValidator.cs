namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Validations;

public class ArticleUpdateDtoValidator : AbstractValidator<ArticleUpdateDto>
{
    public ArticleUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("required | 请输入测评文章的标题")
            .MaximumLength(50).WithMessage("length | 文章标题不应超过50个字符");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("required | 请输入测评文章的内容")
            .MinimumLength(20).WithMessage("length | 文章内容应大于20个字符");

        RuleFor(x => x.CategoryTypeId)
            .NotNull().WithMessage("required | 请选择测评文章的类别")
            .GreaterThan(0).WithMessage("invalid | 不存在的测评分类");

        RuleFor(x => x.GameId)
            .NotNull().WithMessage("required | 请选择您测评的游戏")
            .GreaterThan(0).WithMessage("invalid | 不存在的游戏")
            .LessThan(int.MaxValue).WithMessage("invalid | 不存在的游戏");


        RuleFor(x => x.GameName)
            .NotNull().WithMessage("required | 游戏名不应为空字符串")
            .MaximumLength(50).WithMessage("length | 游戏名过长");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("required | 请勾选您的测评文章状态")
            .Must(x => x == ArticleStatus.Normal || x == ArticleStatus.Draft).WithMessage("invalid | 非法更新测评文章状态");
    }
}