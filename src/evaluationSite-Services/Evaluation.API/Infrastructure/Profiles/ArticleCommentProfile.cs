namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class ArticleCommentProfile : Profile
{
    public ArticleCommentProfile()
    {
        CreateMap<EvaluationComment, ArticleCommentDto>();
        CreateMap<EvaluationComment, ReplyCommentDto>();
    }
}
