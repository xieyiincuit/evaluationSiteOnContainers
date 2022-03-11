namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.DtoModels;

public class PaginatedItemsDtoModel<TEntity> where TEntity : class
{
    public PaginatedItemsDtoModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data, List<UserAvatarDto> other)
    {
        CurrentPage = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int) Math.Ceiling(count / (double) pageSize);
        Data = data;
        UserInfo = other;
    }

    private int CurrentPage { get; }
    private int TotalPages { get; }
    private int PageSize { get; }
    private int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    private IEnumerable<TEntity> Data { get; }
    private List<UserAvatarDto> UserInfo { get; }
}