namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<ArticleAddDto, EvaluationArticle>();
        CreateMap<ArticleUpdateDto, EvaluationArticle>();
        CreateMap<EvaluationArticle, ArticleUpdateDto>();
        CreateMap<EvaluationArticle, ArticleDto>();
    }
}