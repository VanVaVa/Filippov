using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using HRPlatform.Services;
using Moq;
using Xunit;

namespace HRPlatform.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<ILogger<EmployeeService>> _loggerMock;
    private readonly IEmployeeService _service;

    public EmployeeServiceTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<EmployeeService>>();
        _service = new EmployeeService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployeeDto_WhenExists()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PositionId = 1,
            DepartmentId = 1,
            HireDate = DateTime.Today,
            Status = EmployeeStatus.Active,
            Position = new Position { Id = 1, Title = "Developer" },
            Department = new Department { Id = 1, Name = "IT" }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.GetByIdAsync(1, null, UserRole.Admin);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByIdAsync(1, null, UserRole.Admin);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesEmployee()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PositionId = 1,
            DepartmentId = 1,
            HireDate = DateTime.Today,
            Status = EmployeeStatus.Active
        };

        var employee = new Employee
        {
            Id = 1,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PositionId = dto.PositionId,
            DepartmentId = dto.DepartmentId,
            HireDate = dto.HireDate,
            Status = dto.Status
        };

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsUnauthorizedAccessException_WhenEmployeeTriesToAccessOtherEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PositionId = 1,
            DepartmentId = 1,
            HireDate = DateTime.Today,
            Status = EmployeeStatus.Active
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(employee);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetByIdAsync(1, 2, UserRole.Employee));
    }
}

