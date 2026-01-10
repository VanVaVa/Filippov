using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByIdAsync(int id);
    Task<UserAccount?> GetByUsernameAsync(string username);
    Task<UserAccount?> GetByEmployeeIdAsync(int employeeId);
    Task<UserAccount> CreateAsync(UserAccount userAccount);
    Task<UserAccount> UpdateAsync(UserAccount userAccount);
}

