namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

public class ApproveRecordProfile : Profile
{
    public ApproveRecordProfile()
    {
        CreateMap<ApproveRecord, ApproveRecordDto>();
        CreateMap<BannedRecordAddDto, ApproveRecord>();
        CreateMap<ApproveRecordUpdateDto, ApproveRecord>();
    }
}