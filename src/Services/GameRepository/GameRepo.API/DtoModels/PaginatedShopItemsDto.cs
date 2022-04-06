namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;
public class PaginatedShopItemsDto<TEntity> : PaginatedItemsDtoModel<TEntity> where TEntity : class
{
    public PaginatedShopItemsDto(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data, IEnumerable<ArticleShopDto> articles)
        : base(pageIndex, pageSize, count, data)
    {
        Articles = articles;
    }

    public IEnumerable<ArticleShopDto> Articles { get; set; }
}
