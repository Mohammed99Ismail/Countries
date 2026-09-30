using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;

namespace Countries.Services;

public interface ICityService
{
    Task<PagedResult<CityDto>> GetCitiesAsync(CityFilterParams filter, CancellationToken ct = default);
    Task<CityDto> GetCityByIdAsync(int id, CancellationToken ct = default);
    Task<CityDto> CreateCityAsync(CreateCityRequest request, CancellationToken ct = default);
    Task<CityDto> UpdateCityAsync(int id, UpdateCityRequest request, CancellationToken ct = default);
    Task DeleteCityAsync(int id, CancellationToken ct = default);
    Task<PagedResult<CityDto>> GetCitiesByCountryIdAsync(int countryId, PaginationParams pagination, CancellationToken ct = default);
}
