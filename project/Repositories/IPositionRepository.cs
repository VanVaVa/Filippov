using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface IPositionRepository
{
    Task<IEnumerable<Position>> GetAllAsync();
    Task<Position?> GetByIdAsync(int id);
    Task<Position> CreateAsync(Position position);
    Task<Position> UpdateAsync(Position position);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

