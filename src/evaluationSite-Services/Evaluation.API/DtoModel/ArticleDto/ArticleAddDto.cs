namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModel;

public class ArticleAddDto
{
    public int UserId { get; set; }
    public string Title { get; set; }
    public string? DesciptionImage { get; set; }
    public string? ArticleImage { get; set; }
    public string Content { get; set; }
    public string? Description { get; set; }
    public int CategoryTypeId { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; }
}