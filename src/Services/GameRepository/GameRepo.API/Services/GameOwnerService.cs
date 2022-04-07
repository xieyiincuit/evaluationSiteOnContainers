namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameOwnerService : IGameOwnerService
{
    private readonly GameRepoContext _repoDbContext;

    public GameOwnerService(GameRepoContext repoDbContext)
    {
        _repoDbContext = repoDbContext ?? throw new ArgumentNullException(nameof(repoDbContext));
    }

    public async Task<GameOwner> GetGameOwnerRecordAsync(string userId, int gameId)
    {
        return await _repoDbContext.GameOwners
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GameId == gameId && x.UserId == userId);
    }

    public async Task<bool> DeleteGameOwnerRecordAsync(string userId, int gameId)
    {
        var ownerRecordToDelete = await _repoDbContext.GameOwners.FindAsync(userId, gameId);
        if (ownerRecordToDelete == null) return false;
        _repoDbContext.GameOwners.Remove(ownerRecordToDelete);
        return await _repoDbContext.SaveChangesAsync() > 0;
    }

    public async Task AddGameOwnerRecordAsync(string userId, int gameId)
    {
        var gameOwnerRecord = new GameOwner
        {
            GameId = gameId,
            UserId = userId
        };
        await _repoDbContext.AddAsync(gameOwnerRecord);
    }

    public async Task UpdateGameScoreAsync(string userId, int gameId, double gameScore)
    {
        var gameRecord = await _repoDbContext.GameOwners.FindAsync(userId, gameId);
        gameRecord.GameScore = gameScore;
        await _repoDbContext.SaveChangesAsync();
    }

    public async Task<double> CalculateGameScore(int gameId)
    {
        var gameRecords = await _repoDbContext.GameOwners
            .Where(x => x.GameId == gameId && x.GameScore != 0)
            .AsNoTracking()
            .ToListAsync();

        int recordCount = gameRecords.Count;
        double sumScore = 0;

        gameRecords.ForEach(x => sumScore += x.GameScore);
        return sumScore / recordCount;
    }
}