namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class GameInfoProfile : Profile
{
    public GameInfoProfile()
    {
        //从导航属性中mapping, 这需要我们在进行Select时加载导航属性
        CreateMap<GameInfo, GameInfoSmallDto>();

        CreateMap<GameInfo, GameInfoDto>()
           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.GameCategory.CategoryName))
           .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.GameCompany.CompanyName));

        CreateMap<GameInfoAddDto, GameInfo>()
            .ForMember(dest => dest.GameCategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.GameCompanyId, opt => opt.MapFrom(src => src.CompanyId));
        
        CreateMap<GameInfoUpdateDto, GameInfo>()
            .ForMember(dest => dest.GameCategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.GameCompanyId, opt => opt.MapFrom(src => src.CompanyId));
    }
}