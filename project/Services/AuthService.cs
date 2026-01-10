using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace HRPlatform.Services;

public class AuthService : IAuthService
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly HRDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserAccountRepository userAccountRepository,
        IEmployeeRepository employeeRepository,
        HRDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userAccountRepository = userAccountRepository;
        _employeeRepository = employeeRepository;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var userAccount = await _userAccountRepository.GetByUsernameAsync(request.Username);
        if (userAccount == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, userAccount.HashedPassword))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var token = GenerateJwtToken(userAccount);

        _logger.LogInformation("User logged in: {Username}", request.Username);

        return new LoginResponse
        {
            Token = token,
            Username = userAccount.Username,
            Role = userAccount.Role.ToString(),
            EmployeeId = userAccount.EmployeeId
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userAccountRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var existingEmployee = await _employeeRepository.GetByEmailAsync(request.Email);
        if (existingEmployee != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var positionExists = await _context.Positions.AnyAsync(p => p.Id == request.PositionId);
        if (!positionExists)
        {
            var availablePositions = await _context.Positions.Select(p => new { p.Id, p.Title }).ToListAsync();
            throw new ArgumentException($"Invalid position ID: {request.PositionId}. Available positions: {string.Join(", ", availablePositions.Select(p => $"ID {p.Id} - {p.Title}"))}");
        }

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId);
        if (!departmentExists)
        {
            var availableDepartments = await _context.Departments.Select(d => new { d.Id, d.Name }).ToListAsync();
            throw new ArgumentException($"Invalid department ID: {request.DepartmentId}. Available departments: {string.Join(", ", availableDepartments.Select(d => $"ID {d.Id} - {d.Name}"))}");
        }

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PositionId = request.PositionId,
            DepartmentId = request.DepartmentId,
            HireDate = DateTime.UtcNow,
            Status = EmployeeStatus.Active
        };

        var createdEmployee = await _employeeRepository.CreateAsync(employee);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var userAccount = new UserAccount
        {
            Username = request.Username,
            HashedPassword = hashedPassword,
            EmployeeId = createdEmployee.Id,
            Role = UserRole.Employee
        };

        var createdUserAccount = await _userAccountRepository.CreateAsync(userAccount);

        var token = GenerateJwtToken(createdUserAccount);

        _logger.LogInformation("User registered: {Username} - Employee ID: {EmployeeId}", request.Username, createdEmployee.Id);

        return new LoginResponse
        {
            Token = token,
            Username = createdUserAccount.Username,
            Role = createdUserAccount.Role.ToString(),
            EmployeeId = createdUserAccount.EmployeeId
        };
    }

    private string GenerateJwtToken(UserAccount userAccount)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = jwtSettings["Issuer"] ?? "HRPlatform";
        var audience = jwtSettings["Audience"] ?? "HRPlatformUsers";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
            new Claim(ClaimTypes.Name, userAccount.Username),
            new Claim(ClaimTypes.Role, userAccount.Role.ToString()),
            new Claim("EmployeeId", userAccount.EmployeeId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

