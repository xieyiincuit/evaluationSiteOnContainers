namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoParameters;

public class ArticleDtoParameters
{
    private const int MaxPageSize = 20;

    public int MyProperty { get; set; }
    public int CategoryId { get; set; }

    public int PageIndex { get; set; } = 1;

    public string OrderBy { get; set; } = "CreateTime";

    public string Fields { get; set; }

    private int _pageSize = 8;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
