using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class CreateSkillDto
{
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }
}

