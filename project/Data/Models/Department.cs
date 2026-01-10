namespace HRPlatform.Data.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ManagerId { get; set; }

    public Employee? Manager { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}

