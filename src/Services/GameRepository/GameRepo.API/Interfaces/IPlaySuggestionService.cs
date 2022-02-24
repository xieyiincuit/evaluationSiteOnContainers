namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IPlaySuggestionService
{
    Task<List<GamePlaySuggestion>> GetPlaySuggestionsAsync(int pageIndex, int pageSize);
    Task<GamePlaySuggestion> GetPlaySuggestionAsync(int suggestionId);
    Task<int> CountPlaySuggestionsAsync();
    Task<bool> AddPlaySuggestionAsync(GamePlaySuggestion gamePlaySuggestion);
    Task<bool> DeletePlaySuggestionAsync(int suggestionId);
    Task<bool> UpdatePlaySuggestionAsync(GamePlaySuggestion gamePlaySuggestion);
}