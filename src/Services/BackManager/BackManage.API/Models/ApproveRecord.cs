namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models;

[Table("approve_record"), Comment("测评资格申请表")]
public class ApproveRecord
{
    [Key]
    [Column("approve_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    [Column("user_id"), Comment("申请用户id")]
    public string UserId { get; set; }

    [Required, MaxLength(1000)]
    [Column("approve_body"), Comment("测评审批信息正文")]
    public string Body { get; set; }

    [Required]
    [Column("apply_time"), Comment("申请时间")]
    public DateTime ApplyTime { get; set; }

    [Required]
    [Column("approve_status"), Comment("审批状态")]
    public ApproveStatus Status { get; set; }

    [Column("approve_time"), Comment("审批时间")]
    public DateTime? ApproveTime { get; set; }

    [MaxLength(200)]
    [Column("approve_user"), Comment("审批人")]
    public string? ApproveUser { get; set; }

    [Column("a_info_id"), Comment("审批信息id")]
    public int ApproveInfoId { get; set; }
}

public enum ApproveStatus
{
    Rejected = -1,
    Progressing = 0,
    Approved = 1,
}