using Grpc.Core;
using GrpcEvaluation;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Grpc;

public class EvaluationService : EvaluationRepository.EvaluationRepositoryBase
{
    private readonly ILogger<EvaluationService> _logger;
    private readonly EvaluationContext _dbContext;

    public EvaluationService(ILogger<EvaluationService> logger, EvaluationContext dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public override async Task<articleStatisticsResponse> GetArticleStatisticsInfo(articleStatisticsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Begin grpc call from method {Method} for statistics", context.Method);
        var response = new articleStatisticsResponse();

        var allArticleCount = await _dbContext.Articles.CountAsync();
        response.ArticleCount = allArticleCount;

        var allCommentCount = await _dbContext.Comments.CountAsync();
        response.CommentCount = allCommentCount;

        var categories = await _dbContext.Categories.AsNoTracking().ToListAsync();
        foreach (var category in categories)
        {
            response.CategoryMap[category.CategoryType] =
                await _dbContext.Articles.CountAsync(x => x.CategoryTypeId == category.CategoryId);
        }
        context.Status = new Status(StatusCode.OK, "statistics info has been return");
        return response;
    }
}