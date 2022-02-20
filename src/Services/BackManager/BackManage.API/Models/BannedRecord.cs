namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models;

[Table("banned_record"), Comment("用户举报记录表")]
public class BannedRecord
{
    [Key]
    [Column("banned_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    [Column("user_id"), Comment("被举报用户id")]
    public string UserId { get; set; }

    [Required]
    [Column("report_count"), Comment("被举报次数")]
    public int ReportCount { get; set; }

    [Column("banned_time"), Comment("冻结时间")]
    public DateTime? BannedTime { get; set; }

    [MaxLength(200)]
    [Column("approve_user"), Comment("审批人")]
    public string? ApproveUser { get; set; }

    [JsonIgnore, NotMapped]
    public List<BannedInfo> BannedInfos { get; set; }
}