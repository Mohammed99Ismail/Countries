using Countries.Data;
using Countries.Exceptions;
using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;
using Countries.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Countries.Services;

public class CityService : ICityService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateCityRequest> _createValidator;
    private readonly IValidator<UpdateCityRequest> _updateValidator;

    public CityService(
        AppDbContext context,
        IValidator<CreateCityRequest> createValidator,
        IValidator<UpdateCityRequest> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<CityDto>> GetCitiesAsync(CityFilterParams filter, CancellationToken ct = default)
    {
        IQueryable<City> query = _context.Cities
            .Include(c => c.Country)
            .AsNoTracking();

        if (filter.CountryId.HasValue && filter.CountryId.Value > 0)
        {
            query = query.Where(c => c.CountryId == filter.CountryId.Value);
        }

        if (filter.IsCapital.HasValue)
        {
            query = query.Where(c => c.IsCapital == filter.IsCapital.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search) ||
                                     (c.StateOrProvince != null && c.StateOrProvince.ToLower().Contains(search)));
        }

        var isDescending = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = (filter.SortBy?.ToLowerInvariant()) switch
        {
            "population" => isDescending ? query.OrderByDescending(c => c.Population ?? 0) : query.OrderBy(c => c.Population ?? 0),
            "country" => isDescending ? query.OrderByDescending(c => c.Country!.Name) : query.OrderBy(c => c.Country!.Name),
            "createdat" => isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _ => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                CountryId = c.CountryId,
                CountryName = c.Country != null ? c.Country.Name : string.Empty,
                CountryIsoCode = c.Country != null ? c.Country.IsoCode : string.Empty,
                StateOrProvince = c.StateOrProvince,
                Population = c.Population,
                IsCapital = c.IsCapital,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<CityDto>(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<CityDto> GetCityByIdAsync(int id, CancellationToken ct = default)
    {
        var city = await _context.Cities
            .Include(c => c.Country)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (city == null)
        {
            throw new NotFoundException("City", id);
        }

        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            CountryId = city.CountryId,
            CountryName = city.Country != null ? city.Country.Name : string.Empty,
            CountryIsoCode = city.Country != null ? city.Country.IsoCode : string.Empty,
            StateOrProvince = city.StateOrProvince,
            Population = city.Population,
            IsCapital = city.IsCapital,
            CreatedAt = city.CreatedAt,
            UpdatedAt = city.UpdatedAt
        };
    }

    public async Task<CityDto> CreateCityAsync(CreateCityRequest request, CancellationToken ct = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException(errors);
        }

        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == request.CountryId, ct);
        if (country == null)
        {
            throw new NotFoundException("Country", request.CountryId);
        }

        var normalizedName = request.Name.Trim();
        var exists = await _context.Cities.AnyAsync(
            c => c.CountryId == request.CountryId && c.Name.ToLower() == normalizedName.ToLower(), ct);

        if (exists)
        {
            throw new ConflictException($"A city named '{normalizedName}' already exists in country '{country.Name}'.");
        }

        if (request.IsCapital)
        {
            // Unset previous capital of this country if any
            var previousCapitals = await _context.Cities
                .Where(c => c.CountryId == request.CountryId && c.IsCapital)
                .ToListAsync(ct);

            foreach (var prev in previousCapitals)
            {
                prev.IsCapital = false;
            }
        }

        var city = new City
        {
            Name = normalizedName,
            CountryId = request.CountryId,
            StateOrProvince = string.IsNullOrWhiteSpace(request.StateOrProvince) ? null : request.StateOrProvince.Trim(),
            Population = request.Population,
            IsCapital = request.IsCapital,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.Cities.AddAsync(city, ct);
        await _context.SaveChangesAsync(ct);

        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            CountryId = city.CountryId,
            CountryName = country.Name,
            CountryIsoCode = country.IsoCode,
            StateOrProvince = city.StateOrProvince,
            Population = city.Population,
            IsCapital = city.IsCapital,
            CreatedAt = city.CreatedAt,
            UpdatedAt = city.UpdatedAt
        };
    }

    public async Task<CityDto> UpdateCityAsync(int id, UpdateCityRequest request, CancellationToken ct = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException(errors);
        }

        var city = await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (city == null)
        {
            throw new NotFoundException("City", id);
        }

        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == request.CountryId, ct);
        if (country == null)
        {
            throw new NotFoundException("Country", request.CountryId);
        }

        var normalizedName = request.Name.Trim();
        var exists = await _context.Cities.AnyAsync(
            c => c.Id != id && c.CountryId == request.CountryId && c.Name.ToLower() == normalizedName.ToLower(), ct);

        if (exists)
        {
            throw new ConflictException($"Another city named '{normalizedName}' already exists in country '{country.Name}'.");
        }

        if (request.IsCapital && !city.IsCapital)
        {
            // Unset previous capital of this country
            var previousCapitals = await _context.Cities
                .Where(c => c.CountryId == request.CountryId && c.IsCapital && c.Id != id)
                .ToListAsync(ct);

            foreach (var prev in previousCapitals)
            {
                prev.IsCapital = false;
            }
        }

        city.Name = normalizedName;
        city.CountryId = request.CountryId;
        city.StateOrProvince = string.IsNullOrWhiteSpace(request.StateOrProvince) ? null : request.StateOrProvince.Trim();
        city.Population = request.Population;
        city.IsCapital = request.IsCapital;
        city.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            CountryId = city.CountryId,
            CountryName = country.Name,
            CountryIsoCode = country.IsoCode,
            StateOrProvince = city.StateOrProvince,
            Population = city.Population,
            IsCapital = city.IsCapital,
            CreatedAt = city.CreatedAt,
            UpdatedAt = city.UpdatedAt
        };
    }

    public async Task DeleteCityAsync(int id, CancellationToken ct = default)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (city == null)
        {
            throw new NotFoundException("City", id);
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<CityDto>> GetCitiesByCountryIdAsync(int countryId, PaginationParams pagination, CancellationToken ct = default)
    {
        var country = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == countryId, ct);
        if (country == null)
        {
            throw new NotFoundException("Country", countryId);
        }

        var query = _context.Cities
            .Where(c => c.CountryId == countryId)
            .OrderByDescending(c => c.IsCapital)
            .ThenBy(c => c.Name)
            .AsNoTracking();

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                CountryId = c.CountryId,
                CountryName = country.Name,
                CountryIsoCode = country.IsoCode,
                StateOrProvince = c.StateOrProvince,
                Population = c.Population,
                IsCapital = c.IsCapital,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<CityDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }
}
