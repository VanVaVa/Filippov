using HRPlatform.DTO.Reports;
using HRPlatform.Repositories;

namespace HRPlatform.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;

    public ReportService(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeSkillsSummaryDto>> GetEmployeeSkillsSummaryAsync()
    {
        return await _repository.GetEmployeeSkillsSummaryAsync();
    }
}

