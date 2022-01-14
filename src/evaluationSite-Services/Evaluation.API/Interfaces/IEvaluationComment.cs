namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationComment
{
    Task<List<EvaluationComment>> GetArticleComments(int pageIndex, int pageSize, int articleId);
    Task<List<EvaluationComment>> GetUserComments(int userId);
}
