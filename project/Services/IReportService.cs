using HRPlatform.DTO.Reports;

namespace HRPlatform.Services;

public interface IReportService
{
    Task<IEnumerable<EmployeeSkillsSummaryDto>> GetEmployeeSkillsSummaryAsync();
}

