namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models;

[Table("banned_user_link"), Comment("用户举报链接表")]
public class BannedUserLink
{
    [Required]
    [MaxLength(200)]
    [Column("banned_user_id"), Comment("被举报用户id")]
    public string BannedUserId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("check_user_id"), Comment("发起举报用户id")]
    public string CheckUserId { get; set; }
}