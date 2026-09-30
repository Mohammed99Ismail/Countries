using Countries.Models.DTOs.Common;
using Countries.Models.DTOs.Country;

namespace Countries.Services;

public interface ICountryService
{
    Task<PagedResult<CountryDto>> GetCountriesAsync(CountryFilterParams filter, CancellationToken ct = default);
    Task<CountryDetailDto> GetCountryByIdAsync(int id, CancellationToken ct = default);
    Task<CountryDto> CreateCountryAsync(CreateCountryRequest request, CancellationToken ct = default);
    Task<CountryDto> UpdateCountryAsync(int id, UpdateCountryRequest request, CancellationToken ct = default);
    Task DeleteCountryAsync(int id, CancellationToken ct = default);
}
