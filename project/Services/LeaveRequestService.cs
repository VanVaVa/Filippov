using HRPlatform.Data.Models;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Repositories;
using HRPlatform.Services;

namespace HRPlatform.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<LeaveRequestService> _logger;

    public LeaveRequestService(
        ILeaveRequestRepository repository,
        IEmployeeRepository employeeRepository,
        IRedisCacheService cache,
        ILogger<LeaveRequestService> logger)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetAllAsync()
    {
        var requests = await _repository.GetAllAsync();
        return requests.Select(MapToDto);
    }

    public async Task<LeaveRequestDto?> GetByIdAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        return request != null ? MapToDto(request) : null;
    }

    public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, string? idempotencyKey = null)
    {
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var cacheKey = $"idempotency:leave-request:{idempotencyKey}";
            var cached = await _cache.GetAsync<LeaveRequestDto>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Idempotent leave request creation: {IdempotencyKey}", idempotencyKey);
                return cached;
            }
        }

        if (!await _employeeRepository.ExistsAsync(dto.EmployeeId))
            throw new KeyNotFoundException($"Employee with ID {dto.EmployeeId} not found");

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Type = dto.Type,
            Status = LeaveStatus.Pending,
            Comment = dto.Comment
        };

        var created = await _repository.CreateAsync(leaveRequest);
        var result = MapToDto(created);

        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var cacheKey = $"idempotency:leave-request:{idempotencyKey}";
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(24));
        }

        _logger.LogInformation("Leave request created: {LeaveRequestId} for employee {EmployeeId}", created.Id, created.EmployeeId);
        
        return result;
    }

    public async Task<LeaveRequestDto> ApproveAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null)
            throw new KeyNotFoundException($"Leave request with ID {id} not found");

        if (request.Status != LeaveStatus.Pending)
            throw new InvalidOperationException($"Leave request with ID {id} is not in Pending status");

        request.Status = LeaveStatus.Approved;
        var updated = await _repository.UpdateAsync(request);
        
        _logger.LogInformation("Leave request approved: {LeaveRequestId} for employee {EmployeeId}", updated.Id, updated.EmployeeId);
        
        return MapToDto(updated);
    }

    public async Task<LeaveRequestDto> RejectAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null)
            throw new KeyNotFoundException($"Leave request with ID {id} not found");

        if (request.Status != LeaveStatus.Pending)
            throw new InvalidOperationException($"Leave request with ID {id} is not in Pending status");

        request.Status = LeaveStatus.Rejected;
        var updated = await _repository.UpdateAsync(request);
        
        _logger.LogInformation("Leave request rejected: {LeaveRequestId} for employee {EmployeeId}", updated.Id, updated.EmployeeId);
        
        return MapToDto(updated);
    }

    private static LeaveRequestDto MapToDto(LeaveRequest request)
    {
        return new LeaveRequestDto
        {
            Id = request.Id,
            EmployeeId = request.EmployeeId,
            EmployeeName = request.Employee != null 
                ? $"{request.Employee.FirstName} {request.Employee.LastName}" 
                : string.Empty,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Status = request.Status,
            Comment = request.Comment
        };
    }
}

