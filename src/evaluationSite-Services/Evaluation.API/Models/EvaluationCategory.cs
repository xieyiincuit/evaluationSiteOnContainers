namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Models;

public class EvaluationCategory
{
    public int CategoryId { get; set; }

    /// <summary>
    /// 测评类别
    /// </summary>
    public string CategoryType { get; set; }

    [JsonIgnore]
    public bool? IsDeleted { get; set; }
}