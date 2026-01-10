using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Services;

public interface IVacancyService
{
    Task<IEnumerable<VacancyDto>> GetAllAsync();
    Task<IEnumerable<VacancyDto>> GetOpenVacanciesAsync();
    Task<VacancyDto?> GetByIdAsync(int id);
    Task<VacancyDto> CreateAsync(CreateVacancyDto dto);
    Task<VacancyDto> UpdateAsync(int id, UpdateVacancyDto dto);
    Task DeleteAsync(int id);
}

