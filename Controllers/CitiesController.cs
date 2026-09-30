using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;
using Countries.Services;
using Microsoft.AspNetCore.Mvc;

namespace Countries.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    /// <summary>
    /// Gets all cities with pagination and optional filtering by CountryId, IsCapital, or search term.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CityDto>>> GetCities(
        [FromQuery] CityFilterParams filterParams,
        CancellationToken ct)
    {
        var result = await _cityService.GetCitiesAsync(filterParams, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific city by its unique identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> GetCityById(
        [FromRoute] int id,
        CancellationToken ct)
    {
        var result = await _cityService.GetCityByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new city under an existing country.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CityDto>> CreateCity(
        [FromBody] CreateCityRequest request,
        CancellationToken ct)
    {
        var created = await _cityService.CreateCityAsync(request, ct);
        return CreatedAtAction(nameof(GetCityById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing city.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CityDto>> UpdateCity(
        [FromRoute] int id,
        [FromBody] UpdateCityRequest request,
        CancellationToken ct)
    {
        var updated = await _cityService.UpdateCityAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a city.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCity(
        [FromRoute] int id,
        CancellationToken ct)
    {
        await _cityService.DeleteCityAsync(id, ct);
        return NoContent();
    }
}
