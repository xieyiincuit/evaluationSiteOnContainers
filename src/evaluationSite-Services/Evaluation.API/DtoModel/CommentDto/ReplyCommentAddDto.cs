﻿namespace Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModel;

public class ReplyCommentAddDto
{
    public int ArticleId { get; set; }
    public string Content { get; set; }

    public int ReplyUserId { get; set; }
    public int RootCommentId { get; set; }
}