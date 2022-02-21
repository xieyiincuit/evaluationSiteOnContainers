namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Services;

public interface IBannedService
{
    Task<int> CountBannedRecordAsync(BannedStatus status);
    Task<List<BannedRecord>> GetBannedRecordsAsync(int pageIndex, int pageSize, BannedStatus status);
    Task<BannedRecord> GetBannedRecordByIdAsync(int id);
    Task<BannedRecord> GetBannedRecordByUserIdAsync(string userId);
    Task<bool> AddBannedRecordAsync(BannedRecord bannedRecord, string checkUserId);
    Task<bool> UpdateBannedRecordAsync(int id, string checkUserId);
    Task<bool> UpdateBannedStatusRecordAsync(int id, string applyUser);
    Task<bool> DeleteBannedRecordAsync(BannedRecord bannedRecord);

    Task<bool> CheckUserLinkExistAsync(string bannedUserId, string checkUserId);
}