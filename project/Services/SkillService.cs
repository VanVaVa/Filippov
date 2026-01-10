using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;

namespace HRPlatform.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;
    private readonly ILogger<SkillService> _logger;

    public SkillService(ISkillRepository repository, ILogger<SkillService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<SkillDto>> GetAllAsync()
    {
        var skills = await _repository.GetAllAsync();
        return skills.Select(MapToDto);
    }

    public async Task<SkillDto?> GetByIdAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id);
        return skill != null ? MapToDto(skill) : null;
    }

    public async Task<SkillDto> CreateAsync(CreateSkillDto dto)
    {
        var skill = new Skill
        {
            Name = dto.Name,
            Category = dto.Category
        };

        var created = await _repository.CreateAsync(skill);
        _logger.LogInformation("Skill created: {SkillId} - {Name}", created.Id, created.Name);

        return MapToDto(created);
    }

    public async Task<SkillDto> UpdateAsync(int id, UpdateSkillDto dto)
    {
        var skill = await _repository.GetByIdAsync(id);
        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID {id} not found");

        skill.Name = dto.Name;
        skill.Category = dto.Category;

        var updated = await _repository.UpdateAsync(skill);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Skill with ID {id} not found");

        await _repository.DeleteAsync(id);
        _logger.LogInformation("Skill deleted: {SkillId}", id);
    }

    private static SkillDto MapToDto(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category
        };
    }
}

