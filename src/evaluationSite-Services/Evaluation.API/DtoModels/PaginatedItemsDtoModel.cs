namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModels;

public class PaginatedItemsDtoModel<TEntity> where TEntity : class
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public IEnumerable<TEntity> Data { get; private set; }

    public PaginatedItemsDtoModel(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data)
    {
        CurrentPage = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Data = data;
    }
}