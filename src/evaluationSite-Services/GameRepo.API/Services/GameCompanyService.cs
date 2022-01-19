﻿namespace Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Services;

public class GameCompanyService : IGameCompany
{
    private readonly GameRepoContext _repoContext;

    public GameCompanyService(GameRepoContext repoContext)
    {
        _repoContext = repoContext;
    }

    public async Task<bool> AddGameCompanyAsync(GameCompany gameCompany)
    {
        await _repoContext.GameCompanies.AddAsync(gameCompany);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<int> CountCompanyAsync()
    {
        return await _repoContext.GameCompanies.CountAsync();
    }

    public async Task<bool> DeleteGameCompanyAsync(int companyId)
    {
        var company = await _repoContext.GameCompanies.FindAsync(companyId);
        if (company == null) return false;
        _repoContext.GameCompanies.Remove(company);
        return await _repoContext.SaveChangesAsync() > 0;
    }

    public async Task<List<GameCompany>> GetGameCompaniesAsync()
    {
        var companies = await _repoContext.GameCompanies
            .AsNoTracking()
            .ToListAsync();
        return companies;
    }

    public async Task<GameCompany> GetGameCompanyAsync(int companyId)
    {
        var company = await _repoContext.GameCompanies.FindAsync(companyId);
        return company;
    }

    public async Task<bool> UpdeteGameCompanyAsync(GameCompany gameCompany)
    {
        _repoContext.GameCompanies.Update(gameCompany);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}