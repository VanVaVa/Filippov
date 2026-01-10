using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Responses;

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }
}

