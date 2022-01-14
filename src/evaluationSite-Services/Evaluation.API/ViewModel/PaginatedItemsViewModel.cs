namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.ViewModel;

public class PaginatedItemsViewModel<TEntity> where TEntity : class
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; set; }
    public int PageSize { get; private set; }
    public long TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public IEnumerable<TEntity> Data { get; private set; }

    public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    {
        CurrentPage = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Data = data;
    }
}