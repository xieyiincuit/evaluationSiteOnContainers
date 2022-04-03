namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameInfoAdminDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? GameCompanyId { get; set; }
    public string CompanyName { get; set; }
    public int? GameCategoryId { get; set; }
    public string CategoryName { get; set; }
    public DateTime? SellTime { get; set; }
    public long? HotPoints { get; set; }
    public double? AverageScore { get; set; }
}
