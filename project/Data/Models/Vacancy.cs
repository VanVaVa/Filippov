namespace HRPlatform.Data.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public VacancyStatus Status { get; set; }
    public string? SalaryRange { get; set; }

    public Department Department { get; set; } = null!;
    public Position Position { get; set; } = null!;
}

