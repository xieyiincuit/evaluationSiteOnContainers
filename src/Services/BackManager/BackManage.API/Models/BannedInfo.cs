namespace Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models;

[Table("banned_info"), Comment("用户举报内容信息表")]
public class BannedInfo
{
    [Key]
    [Column("b_info_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(1000)]
    [Column("banned_body"), Comment("举报信息正文")]
    public string Body { get; set; }

    [Column("banned_id"), Comment("举报审核外键")]
    public int BannedRecordId { get; set; }

    [NotMapped, JsonIgnore]
    public BannedRecord BannedRecord { get; set; }
}