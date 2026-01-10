using HRPlatform.Data.Context;
using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using HRPlatform.Services;
using Microsoft.EntityFrameworkCore;

namespace HRPlatform.Services;

public class VacancyService : IVacancyService
{
    private readonly IVacancyRepository _repository;
    private readonly HRDbContext _context;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<VacancyService> _logger;
    private const string CacheKey = "vacancies:open";

    public VacancyService(IVacancyRepository repository, HRDbContext context, IRedisCacheService cache, ILogger<VacancyService> logger)
    {
        _repository = repository;
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<VacancyDto>> GetAllAsync()
    {
        var vacancies = await _repository.GetAllAsync();
        return vacancies.Select(MapToDto);
    }

    public async Task<IEnumerable<VacancyDto>> GetOpenVacanciesAsync()
    {
        var cached = await _cache.GetAsync<List<VacancyDto>>(CacheKey);
        if (cached != null)
        {
            return cached;
        }

        var vacancies = await _repository.GetOpenVacanciesAsync();
        var dtos = vacancies.Select(MapToDto).ToList();

        await _cache.SetAsync(CacheKey, dtos, TimeSpan.FromMinutes(5));

        return dtos;
    }

    public async Task<VacancyDto?> GetByIdAsync(int id)
    {
        var vacancy = await _repository.GetByIdAsync(id);
        return vacancy != null ? MapToDto(vacancy) : null;
    }

    public async Task<VacancyDto> CreateAsync(CreateVacancyDto dto)
    {
        var positionExists = await _context.Positions.AnyAsync(p => p.Id == dto.PositionId);
        if (!positionExists)
        {
            var availablePositions = await _context.Positions.Select(p => new { p.Id, p.Title }).ToListAsync();
            throw new ArgumentException($"Invalid position ID: {dto.PositionId}. Available positions: {string.Join(", ", availablePositions.Select(p => $"ID {p.Id} - {p.Title}"))}");
        }

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!departmentExists)
        {
            var availableDepartments = await _context.Departments.Select(d => new { d.Id, d.Name }).ToListAsync();
            throw new ArgumentException($"Invalid department ID: {dto.DepartmentId}. Available departments: {string.Join(", ", availableDepartments.Select(d => $"ID {d.Id} - {d.Name}"))}");
        }

        var vacancy = new Vacancy
        {
            Title = dto.Title,
            Description = dto.Description,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            Status = dto.Status,
            SalaryRange = dto.SalaryRange
        };

        var created = await _repository.CreateAsync(vacancy);
        
        await _cache.RemoveAsync(CacheKey);
        
        _logger.LogInformation("Vacancy created: {VacancyId} - {Title}", created.Id, created.Title);
        
        return MapToDto(created);
    }

    public async Task<VacancyDto> UpdateAsync(int id, UpdateVacancyDto dto)
    {
        var vacancy = await _repository.GetByIdAsync(id);
        if (vacancy == null)
            throw new KeyNotFoundException($"Vacancy with ID {id} not found");

        var positionExists = await _context.Positions.AnyAsync(p => p.Id == dto.PositionId);
        if (!positionExists)
        {
            var availablePositions = await _context.Positions.Select(p => new { p.Id, p.Title }).ToListAsync();
            throw new ArgumentException($"Invalid position ID: {dto.PositionId}. Available positions: {string.Join(", ", availablePositions.Select(p => $"ID {p.Id} - {p.Title}"))}");
        }

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
        if (!departmentExists)
        {
            var availableDepartments = await _context.Departments.Select(d => new { d.Id, d.Name }).ToListAsync();
            throw new ArgumentException($"Invalid department ID: {dto.DepartmentId}. Available departments: {string.Join(", ", availableDepartments.Select(d => $"ID {d.Id} - {d.Name}"))}");
        }

        vacancy.Title = dto.Title;
        vacancy.Description = dto.Description;
        vacancy.DepartmentId = dto.DepartmentId;
        vacancy.PositionId = dto.PositionId;
        vacancy.Status = dto.Status;
        vacancy.SalaryRange = dto.SalaryRange;

        var updated = await _repository.UpdateAsync(vacancy);
        
        await _cache.RemoveAsync(CacheKey);
        
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Vacancy with ID {id} not found");

        await _repository.DeleteAsync(id);
        
        await _cache.RemoveAsync(CacheKey);
        
        _logger.LogInformation("Vacancy deleted: {VacancyId}", id);
    }

    private static VacancyDto MapToDto(Vacancy vacancy)
    {
        return new VacancyDto
        {
            Id = vacancy.Id,
            Title = vacancy.Title,
            Description = vacancy.Description,
            DepartmentId = vacancy.DepartmentId,
            DepartmentName = vacancy.Department?.Name,
            PositionId = vacancy.PositionId,
            PositionTitle = vacancy.Position?.Title,
            Status = vacancy.Status,
            SalaryRange = vacancy.SalaryRange
        };
    }
}

