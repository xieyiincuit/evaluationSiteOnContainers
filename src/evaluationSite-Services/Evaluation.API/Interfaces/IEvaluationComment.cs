namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;

public interface IEvaluationComment
{
    Task<List<EvaluationComment>> GetArticleComments(int articleId);
    Task<List<EvaluationComment>> GetUserComments(int userId);
}
