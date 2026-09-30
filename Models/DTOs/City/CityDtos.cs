using Countries.Models.DTOs.Common;

namespace Countries.Models.DTOs.City;

public class CityFilterParams : PaginationParams
{
    public string? Search { get; set; }
    public int? CountryId { get; set; }
    public bool? IsCapital { get; set; }
    public string? SortBy { get; set; } = "name"; // name, population, country, createdat
    public string? SortOrder { get; set; } = "asc"; // asc, desc
}

public class CityDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int CountryId { get; init; }
    public string CountryName { get; init; } = string.Empty;
    public string CountryIsoCode { get; init; } = string.Empty;
    public string? StateOrProvince { get; init; }
    public long? Population { get; init; }
    public bool IsCapital { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public class CreateCityRequest
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string? StateOrProvince { get; set; }
    public long? Population { get; set; }
    public bool IsCapital { get; set; }
}

public class UpdateCityRequest
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string? StateOrProvince { get; set; }
    public long? Population { get; set; }
    public bool IsCapital { get; set; }
}
