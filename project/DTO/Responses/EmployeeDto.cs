using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Responses;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime HireDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public List<EmployeeSkillDto> Skills { get; set; } = new();
}

public class EmployeeSkillDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public ProficiencyLevel ProficiencyLevel { get; set; }
}

