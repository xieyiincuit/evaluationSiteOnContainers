namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<GameCompanyAddDto, GameCompany>();
        CreateMap<GameCompanyUpdateDto, GameCompany>();
    }
}
