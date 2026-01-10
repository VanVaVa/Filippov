using HRPlatform.DTO.Reports;

namespace HRPlatform.Repositories;

public interface IReportRepository
{
    Task<IEnumerable<EmployeeSkillsSummaryDto>> GetEmployeeSkillsSummaryAsync();
}

