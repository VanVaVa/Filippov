using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class CreateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime HireDate { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
}

