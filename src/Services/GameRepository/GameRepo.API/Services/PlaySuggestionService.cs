namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class PlaySuggestionService : IPlaySuggestionService
{
    private readonly GameRepoContext _repoContext;

    public PlaySuggestionService(GameRepoContext repoContext)
    {
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
    }

    public async Task<bool> AddPlaySuggestionAsync(GamePlaySuggestion gamePlaySuggestion)
    {
        await _repoContext.PlaySuggestions.AddAsync(gamePlaySuggestion);
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

    public async Task<GamePlaySuggestion> GetPlaySuggestionAsync(int suggestionId)
    {
        var suggestion = await _repoContext.PlaySuggestions
            .Include(x => x.GameInfo)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == suggestionId);
        return suggestion;
    }

    public async Task<GamePlaySuggestion> GetPlaySuggestionByGameAsync(int gameId)
    {
        var suggestion = await _repoContext.PlaySuggestions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GameId == gameId);
        return suggestion;
    }

    public async Task<List<GamePlaySuggestion>> GetPlaySuggestionsAsync(int pageIndex, int pageSize)
    {
        var suggestions = await _repoContext.PlaySuggestions
            .Include(x => x.GameInfo)
            .OrderBy(x => x.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return suggestions;
    }

    public async Task<bool> UpdatePlaySuggestionAsync(GamePlaySuggestion gamePlaySuggestion)
    {
        _repoContext.PlaySuggestions.Update(gamePlaySuggestion);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}