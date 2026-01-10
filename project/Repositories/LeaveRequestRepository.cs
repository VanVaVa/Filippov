using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly HRDbContext _context;

    public LeaveRequestRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id);
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequest.Id) ?? leaveRequest;
    }

    public async Task<LeaveRequest> UpdateAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Update(leaveRequest);
        await _context.SaveChangesAsync();
        
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == leaveRequest.Id) ?? leaveRequest;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.LeaveRequests.AnyAsync(lr => lr.Id == id);
    }
}

