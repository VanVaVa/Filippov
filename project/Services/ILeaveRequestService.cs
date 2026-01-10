using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;

namespace HRPlatform.Services;

public interface ILeaveRequestService
{
    Task<IEnumerable<LeaveRequestDto>> GetAllAsync();
    Task<LeaveRequestDto?> GetByIdAsync(int id);
    Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, string? idempotencyKey = null);
    Task<LeaveRequestDto> ApproveAsync(int id);
    Task<LeaveRequestDto> RejectAsync(int id);
}

