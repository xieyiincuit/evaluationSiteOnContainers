namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class GameShopItemProfile : Profile
{
    public GameShopItemProfile()
    {
        CreateMap<GameShopItem, ShopItemDtoToUser>()
            .ForMember(dest => dest.FinalPrice, options => options.MapFrom(src => src.Price * (decimal)(src.Discount / 100)))
            .ForMember(dest => dest.Picture, options => options.MapFrom(src => src.SellPictrue))
            .ForMember(dest => dest.GameId, options => options.MapFrom(src => src.GameInfoId))
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name))
            .ForMember(dest => dest.GamePicture, options => options.MapFrom(src => src.GameInfo.DetailsPicture))
            .ForMember(dest => dest.GameScore, options => options.MapFrom(src => src.GameInfo.AverageScore))
            .ForMember(dest => dest.SellTime, options => options.MapFrom(src => src.GameInfo.SellTime))
            .ForMember(dest => dest.GameCategory, options => options.MapFrom(src => src.GameInfo.GameCategory.CategoryName));

        CreateMap<GameShopItem, ShopItemDetailDto>()
            .ForMember(dest => dest.FinalPrice, options => options.MapFrom(src => src.Price * (decimal)(src.Discount / 100)))
            .ForMember(dest => dest.Picture, options => options.MapFrom(src => src.SellPictrue))
            .ForMember(dest => dest.GameId, options => options.MapFrom(src => src.GameInfoId))
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name))
            .ForMember(dest => dest.GamePicture, options => options.MapFrom(src => src.GameInfo.DetailsPicture))
            .ForMember(dest => dest.GameDescription, options => options.MapFrom(src => src.GameInfo.Description))
            .ForMember(dest => dest.SellTime, options => options.MapFrom(src => src.GameInfo.SellTime))
            .ForMember(dest => dest.GameCategory, options => options.MapFrom(src => src.GameInfo.GameCategory.CategoryName))
            .ForMember(dest => dest.GameIssue, options => options.MapFrom(src => src.GameInfo.GameCompany.CompanyName));

        CreateMap<GameShopItem, ShopItemDtoToAdmin>()
            .ForMember(dest => dest.GameId, options => options.MapFrom(src => src.GameInfoId))
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name))
            .ForMember(dest => dest.Picture, options => options.MapFrom(src => src.SellPictrue))
            .ForMember(dest => dest.TemporaryStopSell, options => options.DoNotAllowNull());

        CreateMap<ShopItemUpdateDto, GameShopItem>();
        CreateMap<ShopItemAddDto, GameShopItem>();
    }
}