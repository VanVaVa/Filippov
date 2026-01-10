using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Responses;

public class PositionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PositionLevel Level { get; set; }
    public string? Description { get; set; }
}

