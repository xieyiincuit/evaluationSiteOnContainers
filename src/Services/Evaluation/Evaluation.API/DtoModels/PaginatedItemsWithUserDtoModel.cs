namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class PaginatedItemsWithUserDtoModel<TEntity> : PaginatedItemsDtoModel<TEntity> where TEntity : class
{
    public PaginatedItemsWithUserDtoModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data, IEnumerable<UserAvatarDto> userInfos)
         : base(pageIndex, pageSize, count, data)
    {
        UserInfo = userInfos;
    }

    public IEnumerable<UserAvatarDto> UserInfo { get; set; }
}