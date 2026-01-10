using HRPlatform.Data.Models;

namespace HRPlatform.DTO.Requests;

public class CreateLeaveRequestDto
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LeaveType Type { get; set; }
    public string? Comment { get; set; }
}

