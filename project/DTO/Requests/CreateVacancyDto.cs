using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class CreateVacancyDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public VacancyStatus Status { get; set; } = VacancyStatus.Open;
    public string? SalaryRange { get; set; }
}

