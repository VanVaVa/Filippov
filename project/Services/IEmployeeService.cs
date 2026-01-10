using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Data.Models;

namespace HRPlatform.Services;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeDto>> GetPagedAsync(int page, int pageSize, string? search, int? departmentId, EmployeeStatus? status, int? currentUserId, UserRole currentUserRole);
    Task<EmployeeDto?> GetByIdAsync(int id, int? currentUserId, UserRole currentUserRole);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto);
    Task DeleteAsync(int id);
    Task AddSkillAsync(int employeeId, AddEmployeeSkillDto dto);
}

