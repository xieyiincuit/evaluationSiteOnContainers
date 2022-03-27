namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels;

public class PaginatedItemsDtoModel<TEntity> where TEntity : class
{
    public PaginatedItemsDtoModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data)
    {
        CurrentPage = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Data = data;
    }

    public int CurrentPage { get; }
    public int TotalPages { get; set; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public IEnumerable<TEntity> Data { get; }
}