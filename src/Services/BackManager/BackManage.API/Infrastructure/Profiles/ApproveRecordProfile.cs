namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

public class ApproveRecordProfile : Profile
{
    public ApproveRecordProfile()
    {
        CreateMap<ApproveRecord, ApproveRecordDto>();
        CreateMap<ApproveRecord, ApproveRecordBodyDto>();
        CreateMap<ApproveRecordAddDto, ApproveRecord>();
        CreateMap<ApproveRecordUpdateDto, ApproveRecord>();
    }
}