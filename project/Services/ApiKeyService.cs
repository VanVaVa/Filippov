using HRPlatform.Repositories;

namespace HRPlatform.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;

    public ApiKeyService(IApiKeyRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        var key = await _repository.GetByKeyAsync(apiKey);
        
        if (key == null)
            return false;

        if (!key.IsActive)
            return false;

        if (key.ExpiryDate < DateTime.UtcNow)
            return false;

        return true;
    }
}

