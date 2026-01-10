using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Responses;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Comment { get; set; }
}

