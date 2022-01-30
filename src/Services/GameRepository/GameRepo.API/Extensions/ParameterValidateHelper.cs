namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Extensions;

public static class ParameterValidateHelper
{
    public static bool IsInvalidPageIndex(int totalCount, int pageSize, int pageIndex)
    {
        if (pageIndex <= 0) return true;

        var maxPageIndex = (int)Math.Ceiling(totalCount / (double)pageSize);
        return pageIndex > maxPageIndex;
    }
}
