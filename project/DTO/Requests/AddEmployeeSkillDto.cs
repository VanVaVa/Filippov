using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class AddEmployeeSkillDto
{
    public int SkillId { get; set; }
    public ProficiencyLevel ProficiencyLevel { get; set; }
}

