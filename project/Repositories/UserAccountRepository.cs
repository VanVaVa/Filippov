using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly HRDbContext _context;

    public UserAccountRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetByIdAsync(int id)
    {
        return await _context.UserAccounts
            .Include(ua => ua.Employee)
            .FirstOrDefaultAsync(ua => ua.Id == id);
    }

    public async Task<UserAccount?> GetByUsernameAsync(string username)
    {
        return await _context.UserAccounts
            .Include(ua => ua.Employee)
            .FirstOrDefaultAsync(ua => ua.Username == username);
    }

    public async Task<UserAccount?> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.UserAccounts
            .Include(ua => ua.Employee)
            .FirstOrDefaultAsync(ua => ua.EmployeeId == employeeId);
    }

    public async Task<UserAccount> CreateAsync(UserAccount userAccount)
    {
        _context.UserAccounts.Add(userAccount);
        await _context.SaveChangesAsync();
        return userAccount;
    }

    public async Task<UserAccount> UpdateAsync(UserAccount userAccount)
    {
        _context.UserAccounts.Update(userAccount);
        await _context.SaveChangesAsync();
        return userAccount;
    }
}

