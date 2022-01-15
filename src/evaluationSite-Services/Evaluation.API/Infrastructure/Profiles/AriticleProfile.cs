namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class AriticleProfile : Profile
{
    public AriticleProfile()
    {
        CreateMap<ArticleAddDto, EvaluationArticle>();
        CreateMap<ArticleUpdateDto, EvaluationArticle>();
        CreateMap<EvaluationArticle, ArticleUpdateDto>();
    }
}
