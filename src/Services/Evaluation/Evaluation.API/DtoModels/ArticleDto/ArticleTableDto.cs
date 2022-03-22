namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ArticleTableDto
{
    public int Id { get; set; }
    public string GameName { get; set; }
    public string Title { get; set; }
    public DateTime CreateTime { get; set; }
    public int? CategoryId { get; set; }
    public ArticleStatus Status { get; set; }
}