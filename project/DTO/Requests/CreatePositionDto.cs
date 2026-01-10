using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class CreatePositionDto
{
    public string Title { get; set; } = string.Empty;
    public PositionLevel Level { get; set; }
    public string? Description { get; set; }
}

