namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class PlaySuggestionProfile : Profile
{
    public PlaySuggestionProfile()
    {
        CreateMap<PlaySuggestionAddDto, PlaySuggestion>();
        CreateMap<PlaySuggestionUpdateDto, PlaySuggestion>();
    }
}
