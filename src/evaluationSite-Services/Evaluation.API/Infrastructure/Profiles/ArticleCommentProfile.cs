namespace Evaluation.API.Infrastructure.Profiles;

public class ArticleCommentProfile : Profile
{
    public ArticleCommentProfile()
    {
        CreateMap<EvaluationComment, ArticleCommentDto>();
    }
}
