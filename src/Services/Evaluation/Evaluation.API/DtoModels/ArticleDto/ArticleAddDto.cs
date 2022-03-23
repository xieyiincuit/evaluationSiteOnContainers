namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ArticleAddDto
{
    public string Title { get; set; }
    public string? ArticleImage { get; set; }
    public string Content { get; set; }
    public string? Description { get; set; }
    public ArticleStatus Status { get; set; }
    public int CategoryTypeId { get; set; }
    public int GameId { get; set; }
}