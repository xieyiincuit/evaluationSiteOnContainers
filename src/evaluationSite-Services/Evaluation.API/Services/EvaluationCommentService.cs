namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;

public class EvaluationCommentService : IEvaluationComment
{
    private readonly EvaluationContext _evaluationContext;

    public EvaluationCommentService(EvaluationContext context)
    {
        _evaluationContext = context;
    }

    public Task<List<EvaluationComment>> GetArticleComments(int articleId)
    {
        throw new NotImplementedException();
    }

    public Task<List<EvaluationComment>> GetUserComments(int userId)
    {
        throw new NotImplementedException();
    }
}
