namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

public class BannedRecordProfile : Profile
{
    public BannedRecordProfile()
    {
        CreateMap<BannedRecord, BannedRecordDto>();
    }
}