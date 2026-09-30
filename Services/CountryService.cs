using Countries.Data;
using Countries.Exceptions;
using Countries.Models.DTOs.Common;
using Countries.Models.DTOs.Country;
using Countries.Models.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Countries.Services;

public class CountryService : ICountryService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateCountryRequest> _createValidator;
    private readonly IValidator<UpdateCountryRequest> _updateValidator;

    public CountryService(
        AppDbContext context,
        IValidator<CreateCountryRequest> createValidator,
        IValidator<UpdateCountryRequest> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<CountryDto>> GetCountriesAsync(CountryFilterParams filter, CancellationToken ct = default)
    {
        IQueryable<Country> query = _context.Countries.AsNoTracking();

        // Filtering by Search query (Name or IsoCode)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search) || c.IsoCode.ToLower().Contains(search));
        }

        // Filtering by Region
        if (!string.IsNullOrWhiteSpace(filter.Region))
        {
            var region = filter.Region.Trim().ToLower();
            query = query.Where(c => c.Region.ToLower() == region);
        }

        // Sorting
        var isDescending = string.Equals(filter.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = (filter.SortBy?.ToLowerInvariant()) switch
        {
            "isocode" => isDescending ? query.OrderByDescending(c => c.IsoCode) : query.OrderBy(c => c.IsoCode),
            "population" => isDescending ? query.OrderByDescending(c => c.Population ?? 0) : query.OrderBy(c => c.Population ?? 0),
            "createdat" => isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _ => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsoCode = c.IsoCode,
                Region = c.Region,
                PhoneCode = c.PhoneCode,
                Currency = c.Currency,
                Population = c.Population,
                CityCount = c.Cities.Count,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<CountryDto>(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<CountryDetailDto> GetCountryByIdAsync(int id, CancellationToken ct = default)
    {
        var country = await _context.Countries
            .AsNoTracking()
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (country == null)
        {
            throw new NotFoundException("Country", id);
        }

        return new CountryDetailDto
        {
            Id = country.Id,
            Name = country.Name,
            IsoCode = country.IsoCode,
            Region = country.Region,
            PhoneCode = country.PhoneCode,
            Currency = country.Currency,
            Population = country.Population,
            CityCount = country.Cities.Count,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt,
            Cities = country.Cities.Select(city => new CitySummaryDto
            {
                Id = city.Id,
                Name = city.Name,
                StateOrProvince = city.StateOrProvince,
                Population = city.Population,
                IsCapital = city.IsCapital
            }).ToList()
        };
    }

    public async Task<CountryDto> CreateCountryAsync(CreateCountryRequest request, CancellationToken ct = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException(errors);
        }

        var normalizedIso = request.IsoCode.Trim().ToUpperInvariant();
        var normalizedName = request.Name.Trim();

        var isoExists = await _context.Countries.AnyAsync(c => c.IsoCode == normalizedIso, ct);
        if (isoExists)
        {
            throw new ConflictException($"A country with ISO code '{normalizedIso}' already exists.");
        }

        var nameExists = await _context.Countries.AnyAsync(c => c.Name.ToLower() == normalizedName.ToLower(), ct);
        if (nameExists)
        {
            throw new ConflictException($"A country with name '{normalizedName}' already exists.");
        }

        var country = new Country
        {
            Name = normalizedName,
            IsoCode = normalizedIso,
            Region = request.Region.Trim(),
            PhoneCode = string.IsNullOrWhiteSpace(request.PhoneCode) ? null : request.PhoneCode.Trim(),
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? null : request.Currency.Trim().ToUpperInvariant(),
            Population = request.Population,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.Countries.AddAsync(country, ct);
        await _context.SaveChangesAsync(ct);

        return new CountryDto
        {
            Id = country.Id,
            Name = country.Name,
            IsoCode = country.IsoCode,
            Region = country.Region,
            PhoneCode = country.PhoneCode,
            Currency = country.Currency,
            Population = country.Population,
            CityCount = 0,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }

    public async Task<CountryDto> UpdateCountryAsync(int id, UpdateCountryRequest request, CancellationToken ct = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BadRequestException(errors);
        }

        var country = await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (country == null)
        {
            throw new NotFoundException("Country", id);
        }

        var normalizedIso = request.IsoCode.Trim().ToUpperInvariant();
        var normalizedName = request.Name.Trim();

        var isoExists = await _context.Countries.AnyAsync(c => c.Id != id && c.IsoCode == normalizedIso, ct);
        if (isoExists)
        {
            throw new ConflictException($"Another country with ISO code '{normalizedIso}' already exists.");
        }

        var nameExists = await _context.Countries.AnyAsync(c => c.Id != id && c.Name.ToLower() == normalizedName.ToLower(), ct);
        if (nameExists)
        {
            throw new ConflictException($"Another country with name '{normalizedName}' already exists.");
        }

        country.Name = normalizedName;
        country.IsoCode = normalizedIso;
        country.Region = request.Region.Trim();
        country.PhoneCode = string.IsNullOrWhiteSpace(request.PhoneCode) ? null : request.PhoneCode.Trim();
        country.Currency = string.IsNullOrWhiteSpace(request.Currency) ? null : request.Currency.Trim().ToUpperInvariant();
        country.Population = request.Population;
        country.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new CountryDto
        {
            Id = country.Id,
            Name = country.Name,
            IsoCode = country.IsoCode,
            Region = country.Region,
            PhoneCode = country.PhoneCode,
            Currency = country.Currency,
            Population = country.Population,
            CityCount = country.Cities.Count,
            CreatedAt = country.CreatedAt,
            UpdatedAt = country.UpdatedAt
        };
    }

    public async Task DeleteCountryAsync(int id, CancellationToken ct = default)
    {
        var country = await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (country == null)
        {
            throw new NotFoundException("Country", id);
        }

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync(ct);
    }
}
