using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface ISkillRepository
{
    Task<IEnumerable<Skill>> GetAllAsync();
    Task<Skill?> GetByIdAsync(int id);
    Task<Skill> CreateAsync(Skill skill);
    Task<Skill> UpdateAsync(Skill skill);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

