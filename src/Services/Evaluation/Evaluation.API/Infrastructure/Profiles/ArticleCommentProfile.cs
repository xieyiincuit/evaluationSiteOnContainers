namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;

public class ArticleCommentProfile : Profile
{
    public ArticleCommentProfile()
    {
        CreateMap<EvaluationComment, ArticleCommentDto>();
        CreateMap<EvaluationComment, ReplyCommentDto>()
            .ForMember(dest => dest.ReplyCommentId, opt => opt.MapFrom(src => src.ReplyCommentId));

        CreateMap<ArticleCommentAddDto, EvaluationComment>();
        CreateMap<ReplyCommentAddDto, EvaluationComment>();
    }
}