namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class GameShopItemProfile : Profile
{
    public GameShopItemProfile()
    {
        CreateMap<GameShopItem, ShopItemDtoToUser>()
            .ForMember(dest => dest.FinalPrice, options => options.MapFrom(src => src.Price * (decimal)src.Discount))
            .ForMember(dest => dest.GameId, options => options.MapFrom(src => src.GameInfoId))
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name))
            .ForMember(dest => dest.Picture, options => options.MapFrom(src => src.GameInfo.RoughPicture));

        CreateMap<GameShopItem, ShopItemDtoToAdmin>()
            .ForMember(dest => dest.GameId, options => options.MapFrom(src => src.GameInfoId))
            .ForMember(dest => dest.GameName, options => options.MapFrom(src => src.GameInfo.Name))
            .ForMember(dest => dest.Picture, options => options.MapFrom(src => src.GameInfo.RoughPicture))
            .ForMember(dest => dest.TemporaryStopSell, options => options.DoNotAllowNull());

        CreateMap<ShopItemUpdateDto, GameShopItem>();
        CreateMap<ShopItemAddDto, GameShopItem>();
    }
}