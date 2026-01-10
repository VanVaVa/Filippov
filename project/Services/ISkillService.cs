using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllAsync();
    Task<SkillDto?> GetByIdAsync(int id);
    Task<SkillDto> CreateAsync(CreateSkillDto dto);
    Task<SkillDto> UpdateAsync(int id, UpdateSkillDto dto);
    Task DeleteAsync(int id);
}

