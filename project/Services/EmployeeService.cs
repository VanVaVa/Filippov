using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HRPlatform.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<EmployeeDto>> GetPagedAsync(int page, int pageSize, string? search, int? departmentId, EmployeeStatus? status, int? currentUserId, UserRole currentUserRole)
    {
        if (currentUserRole == UserRole.Manager && currentUserId.HasValue)
        {
            var currentEmployee = await _repository.GetByIdAsync(currentUserId.Value);
            if (currentEmployee != null)
            {
                if (!departmentId.HasValue || departmentId.Value != currentEmployee.DepartmentId)
                {
                    departmentId = currentEmployee.DepartmentId;
                }
            }
        }
        else if (currentUserRole == UserRole.Employee)
        {
            if (currentUserId.HasValue)
            {
                var employee = await _repository.GetByIdAsync(currentUserId.Value);
                if (employee != null)
                {
                    if (!departmentId.HasValue || employee.DepartmentId == departmentId.Value)
                    {
                        return new PagedResponse<EmployeeDto>
                        {
                            Items = new[] { MapToDto(employee) },
                            Total = 1,
                            Page = page,
                            PageSize = pageSize
                        };
                    }
                }
            }
            return new PagedResponse<EmployeeDto> { Items = Array.Empty<EmployeeDto>(), Total = 0, Page = page, PageSize = pageSize };
        }

        var employees = await _repository.GetPagedAsync(page, pageSize, search, departmentId, status);
        var total = await _repository.GetTotalCountAsync(search, departmentId, status);

        return new PagedResponse<EmployeeDto>
        {
            Items = employees.Select(MapToDto),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, int? currentUserId, UserRole currentUserRole)
    {
        if (currentUserRole == UserRole.Employee && currentUserId.HasValue && currentUserId.Value != id)
        {
            throw new UnauthorizedAccessException("You can only view your own employee record");
        }

        if (currentUserRole == UserRole.Manager && currentUserId.HasValue)
        {
            var currentEmployee = await _repository.GetByIdAsync(currentUserId.Value);
            var targetEmployee = await _repository.GetByIdAsync(id);
            
            if (targetEmployee == null)
                return null;

            if (currentEmployee?.DepartmentId != targetEmployee.DepartmentId)
            {
                throw new UnauthorizedAccessException("You can only view employees in your department");
            }
        }

        var employee = await _repository.GetByIdAsync(id);
        return employee != null ? MapToDto(employee) : null;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PositionId = dto.PositionId,
            DepartmentId = dto.DepartmentId,
            HireDate = dto.HireDate,
            Status = dto.Status
        };

        var created = await _repository.CreateAsync(employee);
        _logger.LogInformation("Employee created: {EmployeeId} - {FirstName} {LastName}", created.Id, created.FirstName, created.LastName);
        
        return MapToDto(created);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.PositionId = dto.PositionId;
        employee.DepartmentId = dto.DepartmentId;
        employee.HireDate = dto.HireDate;
        employee.Status = dto.Status;

        var updated = await _repository.UpdateAsync(employee);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"Employee with ID {id} not found");

        await _repository.DeleteAsync(id);
        _logger.LogInformation("Employee deleted: {EmployeeId}", id);
    }

    public async Task AddSkillAsync(int employeeId, AddEmployeeSkillDto dto)
    {
        if (!await _repository.ExistsAsync(employeeId))
            throw new KeyNotFoundException($"Employee with ID {employeeId} not found");

        await _repository.AddSkillAsync(employeeId, dto.SkillId, dto.ProficiencyLevel);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PositionId = employee.PositionId,
            PositionTitle = employee.Position?.Title,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            HireDate = employee.HireDate,
            Status = employee.Status,
            Skills = employee.EmployeeSkills.Select(es => new EmployeeSkillDto
            {
                SkillId = es.SkillId,
                SkillName = es.Skill?.Name ?? string.Empty,
                ProficiencyLevel = es.ProficiencyLevel
            }).ToList()
        };
    }
}

