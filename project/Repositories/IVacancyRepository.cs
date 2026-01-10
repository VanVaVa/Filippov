using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface IVacancyRepository
{
    Task<Vacancy?> GetByIdAsync(int id);
    Task<IEnumerable<Vacancy>> GetAllAsync();
    Task<IEnumerable<Vacancy>> GetOpenVacanciesAsync();
    Task<Vacancy> CreateAsync(Vacancy vacancy);
    Task<Vacancy> UpdateAsync(Vacancy vacancy);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

