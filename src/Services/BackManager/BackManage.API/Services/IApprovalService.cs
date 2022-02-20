namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Services;

public interface IApprovalService
{
    Task<int> CountApproveRecordByTypeAsync(ApproveStatus status);
    Task<List<ApproveRecord>> GetApproveRecordsAsync(int pageIndex, int pageSize, ApproveStatus status);
    Task<ApproveRecord> GetApproveRecordByIdAsync(int id);
    Task<ApproveRecord> GetApproveRecordByUserIdAsync(string userId);
    Task<bool> AddApproveRecordAsync(ApproveRecord approveRecord);
    Task<bool> UpdateApproveInfoAsync(int id, string body);
    Task<bool> UpdateApproveStatusAsync(int id, ApproveStatus status, string applyUser);
    Task<bool> DeleteApproveRecordAsync(ApproveRecord approveRecord);
}