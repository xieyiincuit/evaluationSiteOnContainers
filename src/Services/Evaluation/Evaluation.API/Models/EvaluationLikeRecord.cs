namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models;

public class EvaluationLikeRecord
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public int ArticleId { get; set; }
    public DateTime CreateTime { get; set; }

    [JsonIgnore]
    public EvaluationArticle EvaluationArticle { get; set; }
}