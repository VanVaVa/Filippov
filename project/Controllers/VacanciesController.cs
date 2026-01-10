using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRPlatform.DTO.Requests;
using HRPlatform.DTO.Responses;
using HRPlatform.Services;

namespace HRPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VacanciesController : ControllerBase
{
    private readonly IVacancyService _service;
    private readonly ILogger<VacanciesController> _logger;

    public VacanciesController(IVacancyService service, ILogger<VacanciesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VacancyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<VacancyDto>>> GetVacancies()
    {
        var vacancies = await _service.GetAllAsync();
        return Ok(vacancies);
    }

    [HttpGet("open")]
    [ProducesResponseType(typeof(IEnumerable<VacancyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<VacancyDto>>> GetOpenVacancies()
    {
        var vacancies = await _service.GetOpenVacanciesAsync();
        return Ok(vacancies);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VacancyDto>> GetVacancy(int id)
    {
        var vacancy = await _service.GetByIdAsync(id);
        if (vacancy == null)
            return NotFound();

        return Ok(vacancy);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VacancyDto>> CreateVacancy([FromBody] CreateVacancyDto dto)
    {
        try
        {
            var vacancy = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetVacancy), new { id = vacancy.Id }, vacancy);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VacancyDto>> UpdateVacancy(int id, [FromBody] UpdateVacancyDto dto)
    {
        try
        {
            var vacancy = await _service.UpdateAsync(id, dto);
            return Ok(vacancy);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteVacancy(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

