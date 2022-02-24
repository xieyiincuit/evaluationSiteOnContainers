namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;

public class GameCompanyService : IGameCompanyService
{
    private readonly GameRepoContext _repoContext;

    public GameCompanyService(GameRepoContext repoContext)
    {
        _repoContext = repoContext ?? throw new ArgumentNullException(nameof(repoContext));
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

    public async Task<List<GameCompany>> GetGameCompaniesAsync(int pageIndex, int pageSize)
    {
        var companies = await _repoContext.GameCompanies
            .OrderBy(x => x.CompanyName)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
        return companies;
    }

    public async Task<GameCompany> GetGameCompanyAsync(int companyId)
    {
        var company = await _repoContext.GameCompanies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == companyId);
        return company;
    }

    public async Task<bool> UpdateGameCompanyAsync(GameCompany gameCompany)
    {
        _repoContext.GameCompanies.Update(gameCompany);
        return await _repoContext.SaveChangesAsync() > 0;
    }
}