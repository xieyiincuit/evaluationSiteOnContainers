namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.DtoModels;

public class GameInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double? AverageScore { get; set; }
    public string? RoughPicture { get; set; }
    public string Issue { get; set; }
    public string CategoryType { get; set; }
}
