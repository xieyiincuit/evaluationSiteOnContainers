namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Models;

[Table("play_suggestion")]
[Comment("游玩游戏配置建议表")]
public class PlaySuggestion
{
    [Key]
    [Column("suggestion_id"), Comment("主键")]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    [Column("operation_system"), Comment("操作系统建议")]
    public string OperationSystem { get; set; }

    [Required, MaxLength(50)]
    [Column("cpu_name"), Comment("CPU型号建议")]
    public string CPUName { get; set; }

    [Required]
    [Column("memory_size"), Comment("内存大小建议")]
    public double MemorySize { get; set; }

    [Required]
    [Column("disk_size"), Comment("磁盘大小建议")]
    public double DiskSize { get; set; }

    [Required]
    [Column("graphics_card"), Comment("显卡型号建议")]
    public string GraphicsCard { get; set; }

    //一对一关系建立
    [ForeignKey("game_id"), Comment("游戏外键id")]
    public GameInfo GameInfo { get; set; }

}