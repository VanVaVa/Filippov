using HRPlatform.Data.Context;
using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRPlatform.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly HRDbContext _context;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository repository, HRDbContext context, ILogger<DepartmentService> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        return departments.Select(MapToDto);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _repository.GetByIdAsync(id);
        return department != null ? MapToDto(department) : null;
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        var existing = await _repository.GetByCodeAsync(dto.Code);
        if (existing != null)
            throw new InvalidOperationException($"Department with code '{dto.Code}' already exists");

        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ManagerId.Value);
            if (!managerExists)
                throw new ArgumentException($"Employee with ID {dto.ManagerId.Value} not found");
        }

        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            ManagerId = dto.ManagerId
        };

        var created = await _repository.CreateAsync(department);
        _logger.LogInformation("Department created: {DepartmentId} - {Name}", created.Id, created.Name);

        return MapToDto(created);
    }

    public async Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _repository.GetByIdAsync(id);
        if (department == null)
            throw new KeyNotFoundException($"Department with ID {id} not found");

        if (dto.Code != department.Code)
        {
            var existing = await _repository.GetByCodeAsync(dto.Code);
            if (existing != null)
                throw new InvalidOperationException($"Department with code '{dto.Code}' already exists");
        }

        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _context.Employees.AnyAsync(e => e.Id == dto.ManagerId.Value);
            if (!managerExists)
                throw new ArgumentException($"Employee with ID {dto.ManagerId.Value} not found");
        }

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.ManagerId = dto.ManagerId;

        var updated = await _repository.UpdateAsync(department);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Department with ID {id} not found");

        var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
        if (hasEmployees)
            throw new InvalidOperationException($"Cannot delete department with ID {id} because it has employees assigned");

        await _repository.DeleteAsync(id);
        _logger.LogInformation("Department deleted: {DepartmentId}", id);
    }

    private static DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ManagerId = department.ManagerId,
            ManagerName = department.Manager != null 
                ? $"{department.Manager.FirstName} {department.Manager.LastName}" 
                : null
        };
    }
}

