namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class GameInfoProfile : Profile
{
    public GameInfoProfile()
    {
        //从导航属性中mapping, 这需要我们在进行Select时加载导航属性
        CreateMap<GameInfo, GameInfoDto>()
            .ForMember(dest => dest.Issue, opt => opt.MapFrom(src => src.GameCompany.CompanyName))
            .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(src => src.GameCategory.CategoryName));

        CreateMap<GameInfoAddDto, GameInfo>()
            .ForMember(dest => dest.GameCategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.GameCompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.PlaySuggestionId, opt => opt.MapFrom(src => src.SuggestionId));

        CreateMap<GameInfoUpdateDto, GameInfo>()
           .ForMember(dest => dest.GameCategoryId, opt => opt.MapFrom(src => src.CategoryId))
           .ForMember(dest => dest.GameCompanyId, opt => opt.MapFrom(src => src.CompanyId))
           .ForMember(dest => dest.PlaySuggestionId, opt => opt.MapFrom(src => src.SuggestionId));
    }
}
