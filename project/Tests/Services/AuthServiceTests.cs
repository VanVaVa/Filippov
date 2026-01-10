using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.Repositories;
using HRPlatform.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace HRPlatform.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserAccountRepository> _repositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly IAuthService _service;

    public AuthServiceTests()
    {
        _repositoryMock = new Mock<IUserAccountRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(x => x["Secret"]).Returns("YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong");
        jwtSection.Setup(x => x["Issuer"]).Returns("HRPlatform");
        jwtSection.Setup(x => x["Audience"]).Returns("HRPlatformUsers");
        jwtSection.Setup(x => x["ExpiryMinutes"]).Returns("60");

        _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

        _service = new AuthService(_repositoryMock.Object, _configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ReturnsLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "password123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var userAccount = new UserAccount
        {
            Id = 1,
            Username = "testuser",
            HashedPassword = hashedPassword,
            EmployeeId = 1,
            Role = UserRole.Admin
        };

        _repositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync(userAccount);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("Admin", result.Role);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "password123"
        };

        _repositoryMock.Setup(r => r.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((UserAccount?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var userAccount = new UserAccount
        {
            Id = 1,
            Username = "testuser",
            HashedPassword = hashedPassword,
            EmployeeId = 1,
            Role = UserRole.Admin
        };

        _repositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync(userAccount);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(request));
    }
}

