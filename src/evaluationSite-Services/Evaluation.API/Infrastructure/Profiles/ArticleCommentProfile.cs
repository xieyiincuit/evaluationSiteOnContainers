namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class ArticleCommentProfile : Profile
{
    public ArticleCommentProfile()
    {
        CreateMap<EvaluationComment, ArticleCommentDto>();
        CreateMap<EvaluationComment, ReplyCommentDto>();

        CreateMap<ArticleCommentAddDto, EvaluationComment>();
        CreateMap<ReplyCommentAddDto, EvaluationComment>();
    }
}