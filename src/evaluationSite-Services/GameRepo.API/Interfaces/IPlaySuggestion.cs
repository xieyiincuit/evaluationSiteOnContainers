namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IPlaySuggestion
{
    Task<List<PlaySuggestion>> GetPlaySuggestionsAsync();
    Task<PlaySuggestion> GetPlaySuggestionAsync(int suggestionId);
    Task<int> CountPlaySuggestionsAsync();
    Task<bool> AddPlaySuggestionAsync(PlaySuggestion playSuggestion);
    Task<bool> DeletePlaySuggestionAsync(int suggestionId);
    Task<bool> UpdatePlaySuggestionAsync(PlaySuggestion playSuggestion);
}
