using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;
using Countries.Models.DTOs.Country;
using Countries.Services;
using Microsoft.AspNetCore.Mvc;

namespace Countries.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly ICityService _cityService;

    public CountriesController(ICountryService countryService, ICityService cityService)
    {
        _countryService = countryService;
        _cityService = cityService;
    }

    /// <summary>
    /// Gets all countries with pagination, search, and region filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CountryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CountryDto>>> GetCountries(
        [FromQuery] CountryFilterParams filterParams,
        CancellationToken ct)
    {
        var result = await _countryService.GetCountriesAsync(filterParams, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific country by its unique identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CountryDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CountryDetailDto>> GetCountryById(
        [FromRoute] int id,
        CancellationToken ct)
    {
        var result = await _countryService.GetCountryByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new country.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CountryDto>> CreateCountry(
        [FromBody] CreateCountryRequest request,
        CancellationToken ct)
    {
        var created = await _countryService.CreateCountryAsync(request, ct);
        return CreatedAtAction(nameof(GetCountryById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing country.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CountryDto>> UpdateCountry(
        [FromRoute] int id,
        [FromBody] UpdateCountryRequest request,
        CancellationToken ct)
    {
        var updated = await _countryService.UpdateCountryAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a country and its associated cities.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCountry(
        [FromRoute] int id,
        CancellationToken ct)
    {
        await _countryService.DeleteCountryAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Gets all cities for a specific country with pagination.
    /// </summary>
    [HttpGet("{id:int}/cities")]
    [ProducesResponseType(typeof(PagedResult<CityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<CityDto>>> GetCitiesByCountryId(
        [FromRoute] int id,
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var result = await _cityService.GetCitiesByCountryIdAsync(id, pagination, ct);
        return Ok(result);
    }
}
