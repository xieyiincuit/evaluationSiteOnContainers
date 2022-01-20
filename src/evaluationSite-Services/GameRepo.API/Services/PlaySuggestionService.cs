namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class PlaySuggestionService : IPlaySuggestionService
{
    private readonly GameRepoContext _repoContext;

    public PlaySuggestionService(GameRepoContext repoContext)
    {
        _repoContext = repoContext;
    }

    public async Task<bool> AddPlaySuggestionAsync(PlaySuggestion playSuggestion)
    {
        await _repoContext.PlaySuggestions.AddAsync(playSuggestion);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountPlaySuggestionsAsync()
    {
        return await _repoContext.PlaySuggestions.CountAsync();
    }

    public async Task<bool> DeletePlaySuggestionAsync(int suggestionId)
    {
        var suggestion = await _repoContext.PlaySuggestions.FindAsync(suggestionId);
        if (suggestion == null) return false;

        _repoContext.PlaySuggestions.Remove(suggestion);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<PlaySuggestion> GetPlaySuggestionAsync(int suggestionId)
    {
        var suggestion = await _repoContext.PlaySuggestions
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == suggestionId);
        return suggestion;
    }

    public async Task<List<PlaySuggestion>> GetPlaySuggestionsAsync(int pageIndex, int pageSize)
    {
        var suggestions = await _repoContext.PlaySuggestions
            .OrderBy(x => x.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return suggestions;
    }

    public async Task<bool> UpdatePlaySuggestionAsync(PlaySuggestion playSuggestion)
    {
        _repoContext.PlaySuggestions.Update(playSuggestion);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}
