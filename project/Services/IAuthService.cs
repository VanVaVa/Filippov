using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
}

