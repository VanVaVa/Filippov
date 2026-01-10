namespace HRPlatform.Data.Models;

public class EmployeeSkill
{
    public int EmployeeId { get; set; }
    public int SkillId { get; set; }
    public ProficiencyLevel ProficiencyLevel { get; set; }

    public Employee Employee { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

