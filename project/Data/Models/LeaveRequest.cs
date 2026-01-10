namespace HRPlatform.Data.Models;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Comment { get; set; }

    public Employee Employee { get; set; } = null!;
}

