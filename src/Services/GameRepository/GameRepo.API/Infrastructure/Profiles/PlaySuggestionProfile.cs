namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class PlaySuggestionProfile : Profile
{
    public PlaySuggestionProfile()
    {
        CreateMap<PlaySuggestionAddDto, GamePlaySuggestion>();
        CreateMap<PlaySuggestionUpdateDto, GamePlaySuggestion>();

        CreateMap<GamePlaySuggestion, PlaySuggestionDto>()
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name));
    }
}