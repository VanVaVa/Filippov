namespace HRPlatform.DTO.Reports;

public class EmployeeSkillsSummaryDto
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? PositionTitle { get; set; }
    public int SkillCount { get; set; }
    public int AdvancedSkillsCount { get; set; }
    public int IntermediateSkillsCount { get; set; }
    public int BasicSkillsCount { get; set; }
    public string? SkillsList { get; set; }
}

