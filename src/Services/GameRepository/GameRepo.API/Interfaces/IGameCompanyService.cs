namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameCompanyService
{
    Task<List<GameCompany>> GetGameCompaniesAsync(int pageIndex, int pageSize);
    Task<List<GameCompany>> GetAllGameCompaniesAsync();
    Task<GameCompany> GetGameCompanyAsync(int companyId);
    Task<int> CountCompanyAsync();
    Task<bool> AddGameCompanyAsync(GameCompany gameCompany);
    Task<bool> UpdateGameCompanyAsync(GameCompany gameCompany);
    Task<bool> DeleteGameCompanyAsync(int companyId);
}