using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class UpdateSkillDto
{
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }
}

