namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Profiles;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryAddDto, EvaluationCategory>();
        CreateMap<CategoryUpdateDto, EvaluationCategory>();
    }
}
