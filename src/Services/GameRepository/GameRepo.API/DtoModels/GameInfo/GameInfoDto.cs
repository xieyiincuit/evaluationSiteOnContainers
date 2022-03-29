namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? DetailsPicture { get; set; }
    public string? RoughPicture { get; set; }
    public double? AverageScore { get; set; }
    public DateTime? SellTime { get; set; }
    public string SupportPlatform { get; set; }
    public string CompanyName { get; set; }
    public string CategoryName { get; set; }
}
