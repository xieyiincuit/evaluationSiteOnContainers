namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IPlaySuggestionService
{
    Task<List<PlaySuggestion>> GetPlaySuggestionsAsync(int pageIndex, int pageSize);
    Task<PlaySuggestion> GetPlaySuggestionAsync(int suggestionId);
    Task<int> CountPlaySuggestionsAsync();
    Task<bool> AddPlaySuggestionAsync(PlaySuggestion playSuggestion);
    Task<bool> DeletePlaySuggestionAsync(int suggestionId);
    Task<bool> UpdatePlaySuggestionAsync(PlaySuggestion playSuggestion);
}
