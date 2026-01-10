using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Services;

public interface IPositionService
{
    Task<IEnumerable<PositionDto>> GetAllAsync();
    Task<PositionDto?> GetByIdAsync(int id);
    Task<PositionDto> CreateAsync(CreatePositionDto dto);
    Task<PositionDto> UpdateAsync(int id, UpdatePositionDto dto);
    Task DeleteAsync(int id);
}

