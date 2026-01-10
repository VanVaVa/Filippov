using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRPlatform.DTO.Reports;
using HRPlatform.Services;

namespace HRPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("employee-skills-summary")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeSkillsSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<EmployeeSkillsSummaryDto>>> GetEmployeeSkillsSummary()
    {
        var summary = await _service.GetEmployeeSkillsSummaryAsync();
        return Ok(summary);
    }
}

