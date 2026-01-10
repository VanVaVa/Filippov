using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Responses;

public class VacancyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public VacancyStatus Status { get; set; }
    public string? SalaryRange { get; set; }
}

