namespace HRPlatform.Data.Models;

public class Position
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PositionLevel Level { get; set; }
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}

