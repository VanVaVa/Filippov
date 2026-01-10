using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<ApiKey> CreateAsync(ApiKey apiKey);
}

