namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Services;

public class ApproveService : IApprovalService
{
    private readonly BackManageContext _dbContext;

    public ApproveService(BackManageContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> CountApproveRecordByTypeAsync(ApproveStatus status)
    {
        return await _dbContext.ApproveRecords.CountAsync(x => x.Status == status);
    }

    public async Task<List<ApproveRecord>> GetApproveRecordsAsync(int pageIndex, int pageSize, ApproveStatus status)
    {
        var approveList = await _dbContext.ApproveRecords
            .AsNoTracking()
            .Where(x => x.Status == status)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(x => x.ApplyTime)
            .ToListAsync();
        return approveList;
    }

    public async Task<ApproveRecord> GetApproveRecordByIdAsync(int id)
    {
        return await _dbContext.ApproveRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ApproveRecord> GetApproveRecordByUserIdAsync(string userId)
    {
        return await _dbContext.ApproveRecords.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> AddApproveRecordAsync(ApproveRecord approveRecord)
    {
        await _dbContext.ApproveRecords.AddAsync(approveRecord);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateApproveInfoAsync(int id, string body)
    {
        var updateEntity = await _dbContext.ApproveRecords.FindAsync(id);
        updateEntity.Body = body;
        if (updateEntity.Status == ApproveStatus.Rejected)
        {
            updateEntity.Status = ApproveStatus.Progressing;
        }
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateApproveStatusAsync(int id, ApproveStatus status, string applyUser)
    {
        var updateEntity = await _dbContext.ApproveRecords.FindAsync(id);
        updateEntity.Status = status;
        updateEntity.ApproveTime = DateTime.Now.ToLocalTime();
        updateEntity.ApproveUser = applyUser;
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteApproveRecordAsync(ApproveRecord approveRecord)
    {
        _dbContext.ApproveRecords.Remove(approveRecord);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}