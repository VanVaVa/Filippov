using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;

namespace HRPlatform.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _repository;
    private readonly ILogger<PositionService> _logger;

    public PositionService(IPositionRepository repository, ILogger<PositionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<PositionDto>> GetAllAsync()
    {
        var positions = await _repository.GetAllAsync();
        return positions.Select(MapToDto);
    }

    public async Task<PositionDto?> GetByIdAsync(int id)
    {
        var position = await _repository.GetByIdAsync(id);
        return position != null ? MapToDto(position) : null;
    }

    public async Task<PositionDto> CreateAsync(CreatePositionDto dto)
    {
        var position = new Position
        {
            Title = dto.Title,
            Level = dto.Level,
            Description = dto.Description
        };

        var created = await _repository.CreateAsync(position);
        _logger.LogInformation("Position created: {PositionId} - {Title}", created.Id, created.Title);

        return MapToDto(created);
    }

    public async Task<PositionDto> UpdateAsync(int id, UpdatePositionDto dto)
    {
        var position = await _repository.GetByIdAsync(id);
        if (position == null)
            throw new KeyNotFoundException($"Position with ID {id} not found");

        position.Title = dto.Title;
        position.Level = dto.Level;
        position.Description = dto.Description;

        var updated = await _repository.UpdateAsync(position);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Position with ID {id} not found");

        await _repository.DeleteAsync(id);
        _logger.LogInformation("Position deleted: {PositionId}", id);
    }

    private static PositionDto MapToDto(Position position)
    {
        return new PositionDto
        {
            Id = position.Id,
            Title = position.Title,
            Level = position.Level,
            Description = position.Description
        };
    }
}

