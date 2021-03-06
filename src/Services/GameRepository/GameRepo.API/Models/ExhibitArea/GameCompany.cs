namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;

[Table("game_company")]
[Comment("发行公司")]
public class GameCompany
{
    [Key]
    [Column("company_id")]
    [Comment("主键")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("company_name")]
    [Comment("游戏发行公司名")]
    public string CompanyName { get; set; }

    [Column("is_deleted")]
    [Comment("逻辑删除")]
    [JsonIgnore]
    public bool? IsDeleted { get; set; }
}