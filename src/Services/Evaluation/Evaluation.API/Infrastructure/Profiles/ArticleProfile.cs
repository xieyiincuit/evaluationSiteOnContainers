namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<ArticleAddDto, EvaluationArticle>();
        CreateMap<ArticleUpdateDto, EvaluationArticle>();
        CreateMap<EvaluationArticle, ArticleUpdateDto>();
        CreateMap<EvaluationArticle, ArticleDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.NickName));
        CreateMap<EvaluationArticle, ArticleSmallDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.NickName));

        CreateMap<EvaluationArticle, ArticleShopDto>();
    }
}