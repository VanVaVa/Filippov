namespace HRPlatform.Services;

public interface IApiKeyService
{
    Task<bool> ValidateApiKeyAsync(string apiKey);
}

