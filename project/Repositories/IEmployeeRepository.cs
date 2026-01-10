using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetPagedAsync(int page, int pageSize, string? search, int? departmentId, EmployeeStatus? status);
    Task<int> GetTotalCountAsync(string? search, int? departmentId, EmployeeStatus? status);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddSkillAsync(int employeeId, int skillId, ProficiencyLevel proficiencyLevel);
}

