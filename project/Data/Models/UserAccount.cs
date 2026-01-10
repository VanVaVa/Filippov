namespace HRPlatform.Data.Models;

public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public UserRole Role { get; set; }

    public Employee Employee { get; set; } = null!;
}

