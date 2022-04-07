namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;
public class PlaySuggestionDto
{
    public int Id { get; set; }

    public string OperationSystem { get; set; }

    public string CPUName { get; set; }

    public double MemorySize { get; set; }

    public double DiskSize { get; set; }

    public string GraphicsCard { get; set; }

    public string GameName { get; set; }
}
