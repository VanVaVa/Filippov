using HRPlatform.Data.Models;

namespace HRPlatform.Data.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime HireDate { get; set; }
    public EmployeeStatus Status { get; set; }

    public Position Position { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public UserAccount? UserAccount { get; set; }
}

