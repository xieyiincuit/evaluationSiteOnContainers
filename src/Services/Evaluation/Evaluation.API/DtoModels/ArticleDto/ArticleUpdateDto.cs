namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ArticleUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? ArticleImage { get; set; }
    public string Content { get; set; }
    public ArticleStatus Status { get; set; }
    public string? Description { get; set; }
    public int CategoryTypeId { get; set; }
    public int GameId { get; set; }
}