using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;
using HRPlatform.Repositories;
using Xunit;

namespace HRPlatform.Tests.Repositories;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly HRDbContext _context;
    private readonly IEmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<HRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HRDbContext(options);
        _repository = new EmployeeRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var department = new Department { Id = 1, Name = "IT", Code = "IT" };
        var position = new Position { Id = 1, Title = "Developer", Level = PositionLevel.Middle };
        
        _context.Departments.Add(department);
        _context.Positions.Add(position);
        _context.SaveChanges();

        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PositionId = 1,
            DepartmentId = 1,
            HireDate = DateTime.Today.AddYears(-1),
            Status = EmployeeStatus.Active
        };

        _context.Employees.Add(employee);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee_WhenExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PositionId = 1,
            DepartmentId = 1,
            HireDate = DateTime.Today,
            Status = EmployeeStatus.Active
        };

        // Act
        var result = await _repository.CreateAsync(employee);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEmployee()
    {
        // Arrange
        var employee = await _repository.GetByIdAsync(1);
        Assert.NotNull(employee);
        employee.FirstName = "Updated";

        // Act
        var result = await _repository.UpdateAsync(employee);

        // Assert
        Assert.Equal("Updated", result.FirstName);
        var updated = await _repository.GetByIdAsync(1);
        Assert.Equal("Updated", updated!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_DeletesEmployee()
    {
        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var result = await _repository.GetByIdAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenExists()
    {
        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenNotExists()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

