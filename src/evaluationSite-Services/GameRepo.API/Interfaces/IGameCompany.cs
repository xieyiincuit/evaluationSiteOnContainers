﻿namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Interfaces;

public interface IGameCompany
{
    Task<List<GameCompany>> GetGameCompaniesAsync();
    Task<GameCompany> GetGameCompanyAsync(int companyId);
    Task<int> CountCompanyAsync();
    Task<bool> AddGameCompanyAsync(GameCompany gameCompany);
    Task<bool> UpdeteGameCompanyAsync(GameCompany gameCompany);
    Task<bool> DeleteGameCompanyAsync(int companyId);
}