using Countries.Models.DTOs.Common;

namespace Countries.Models.DTOs.Country;

public class CountryFilterParams : PaginationParams
{
    public string? Search { get; set; }
    public string? Region { get; set; }
    public string? SortBy { get; set; } = "name"; // name, isocode, population, createdat
    public string? SortOrder { get; set; } = "asc"; // asc, desc
}

public class CountryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string IsoCode { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string? PhoneCode { get; init; }
    public string? Currency { get; init; }
    public long? Population { get; init; }
    public int CityCount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public class CountryDetailDto : CountryDto
{
    public IReadOnlyList<CitySummaryDto> Cities { get; init; } = Array.Empty<CitySummaryDto>();
}

public class CitySummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? StateOrProvince { get; init; }
    public long? Population { get; init; }
    public bool IsCapital { get; init; }
}

public class CreateCountryRequest
{
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? PhoneCode { get; set; }
    public string? Currency { get; set; }
    public long? Population { get; set; }
}

public class UpdateCountryRequest
{
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? PhoneCode { get; set; }
    public string? Currency { get; set; }
    public long? Population { get; set; }
}
