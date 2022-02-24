namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Services;

public class BannedService : IBannedService
{
    private readonly BackManageContext _dbContext;

    public BannedService(BackManageContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> CountBannedRecordAsync(BannedStatus status)
    {
        return await _dbContext.BannedRecords.CountAsync(x => x.Status == status);
    }

    public async Task<List<BannedRecord>> GetBannedRecordsAsync(int pageIndex, int pageSize, BannedStatus status)
    {
        var bannedRecords = await _dbContext.BannedRecords
            .AsNoTracking()
            .Where(x => x.Status == status)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderByDescending(x => x.ReportCount)
            .ToListAsync();
        return bannedRecords;
    }

    public async Task<BannedRecord> GetBannedRecordByIdAsync(int id)
    {
        return await _dbContext.BannedRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BannedRecord> GetBannedRecordByUserIdAsync(string userId)
    {
        return await _dbContext.BannedRecords.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> AddBannedRecordAsync(BannedRecord bannedRecord, string checkUserId)
    {
        await _dbContext.BannedRecords.AddAsync(bannedRecord);
        await _dbContext.BannedUserLinks.AddAsync(new BannedUserLink
            {BannedUserId = bannedRecord.UserId, CheckUserId = checkUserId});

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateBannedRecordAsync(int id, string checkUserId)
    {
        var updateEntity = await _dbContext.BannedRecords.FindAsync(id);
        updateEntity.ReportCount += 1;

        var userBanLink = new BannedUserLink {BannedUserId = updateEntity.UserId, CheckUserId = checkUserId};
        await _dbContext.BannedUserLinks.AddAsync(userBanLink);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateBannedStatusRecordAsync(int id, string applyUser)
    {
        var updateEntity = await _dbContext.BannedRecords.FindAsync(id);
        updateEntity.BannedTime = DateTime.Now.ToLocalTime();
        updateEntity.Status = BannedStatus.Banned;
        updateEntity.ApproveUser = applyUser;
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteBannedRecordAsync(BannedRecord bannedRecord)
    {
        var linkRecord = await _dbContext.BannedUserLinks.AsNoTracking()
            .Where(x => x.BannedUserId == bannedRecord.UserId).ToListAsync();

        _dbContext.BannedRecords.Remove(bannedRecord);
        _dbContext.BannedUserLinks.RemoveRange(linkRecord);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> CheckUserLinkExistAsync(string bannedUserId, string checkUserId)
    {
        return await _dbContext.BannedUserLinks.FindAsync(bannedUserId, checkUserId) != null;
    }
}