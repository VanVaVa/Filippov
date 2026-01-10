using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;
using HRPlatform.Repositories;
using Xunit;

namespace HRPlatform.Tests.Repositories;

public class VacancyRepositoryTests : IDisposable
{
    private readonly HRDbContext _context;
    private readonly IVacancyRepository _repository;

    public VacancyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<HRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HRDbContext(options);
        _repository = new VacancyRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var department = new Department { Id = 1, Name = "IT", Code = "IT" };
        var position = new Position { Id = 1, Title = "Developer", Level = PositionLevel.Middle };
        
        _context.Departments.Add(department);
        _context.Positions.Add(position);
        _context.SaveChanges();

        var vacancy = new Vacancy
        {
            Id = 1,
            Title = "Senior Developer",
            Description = "Looking for a senior developer",
            DepartmentId = 1,
            PositionId = 1,
            Status = VacancyStatus.Open,
            SalaryRange = "$100k-$150k"
        };

        _context.Vacancies.Add(vacancy);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsVacancy_WhenExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Senior Developer", result.Title);
    }

    [Fact]
    public async Task GetOpenVacanciesAsync_ReturnsOnlyOpenVacancies()
    {
        // Arrange
        var closedVacancy = new Vacancy
        {
            Title = "Closed Position",
            Description = "Closed",
            DepartmentId = 1,
            PositionId = 1,
            Status = VacancyStatus.Closed
        };
        _context.Vacancies.Add(closedVacancy);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetOpenVacanciesAsync();

        // Assert
        Assert.All(result, v => Assert.Equal(VacancyStatus.Open, v.Status));
    }

    [Fact]
    public async Task CreateAsync_CreatesVacancy()
    {
        // Arrange
        var vacancy = new Vacancy
        {
            Title = "New Position",
            Description = "New",
            DepartmentId = 1,
            PositionId = 1,
            Status = VacancyStatus.Open
        };

        // Act
        var result = await _repository.CreateAsync(vacancy);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

