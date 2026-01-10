namespace HRPlatform.DTO.Requests;

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
}

