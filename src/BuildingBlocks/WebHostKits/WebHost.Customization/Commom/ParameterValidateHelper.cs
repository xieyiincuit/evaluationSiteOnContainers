namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.WebHost.Customization;

public static class ParameterValidateHelper
{
    /// <summary>
    ///     非法分页判断函数
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns>非法返回True</returns>
    public static bool IsInvalidPageIndex(long totalCount, int pageSize, int pageIndex)
    {
        if (pageIndex <= 0) return true;

        var maxPageIndex = (int)Math.Ceiling(totalCount / (double)pageSize);
        return pageIndex > maxPageIndex;
    }
}