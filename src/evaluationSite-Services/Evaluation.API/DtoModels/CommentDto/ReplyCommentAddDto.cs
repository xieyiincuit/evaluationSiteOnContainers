namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModels;

public class ReplyCommentAddDto
{
    public int ArticleId { get; set; }
    public string Content { get; set; }

    public int ReplyUserId { get; set; }
    public int ReplyCommentId { get; set; }
    public int RootCommentId { get; set; }
}
