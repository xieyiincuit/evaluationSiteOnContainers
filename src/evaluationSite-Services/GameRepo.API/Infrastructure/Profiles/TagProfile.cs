namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<GameTagAddDto, GameTag>();
        CreateMap<GameTagUpdateDto, GameTag>();
    }
}
