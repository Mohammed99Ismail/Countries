using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;
using Countries.Models.DTOs.Country;
using Xunit;

namespace Countries.Tests;

public class CitiesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CitiesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCities_ReturnsPaginatedList()
    {
        var response = await _client.GetAsync("/api/cities?pageNumber=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CityDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.Equal(1, paged.PageNumber);
        Assert.Equal(5, paged.PageSize);
        Assert.True(paged.TotalCount > 10);
        Assert.Equal(5, paged.Items.Count);
    }

    [Fact]
    public async Task GetCities_WithSearchFilter_ReturnsMatchingCities()
    {
        var response = await _client.GetAsync("/api/cities?search=Tokyo");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CityDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.Contains(paged.Items, c => c.Name == "Tokyo");
    }

    [Fact]
    public async Task GetCities_WithCapitalFilter_ReturnsOnlyCapitals()
    {
        var response = await _client.GetAsync("/api/cities?isCapital=true&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CityDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.NotEmpty(paged.Items);
        Assert.All(paged.Items, c => Assert.True(c.IsCapital));
    }

    [Fact]
    public async Task GetCityById_ValidId_ReturnsCityWithCountryInfo()
    {
        var response = await _client.GetAsync("/api/cities/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var city = await response.Content.ReadFromJsonAsync<CityDto>(JsonOptions);
        Assert.NotNull(city);
        Assert.Equal(1, city.Id);
        Assert.NotEmpty(city.Name);
        Assert.NotEmpty(city.CountryName);
        Assert.NotEmpty(city.CountryIsoCode);
    }

    [Fact]
    public async Task GetCityById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/cities/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCity_ValidRequest_ReturnsCreatedCity()
    {
        var request = new CreateCityRequest
        {
            Name = "Kobe Test",
            CountryId = 1, // Japan
            StateOrProvince = "Hyogo",
            Population = 1520000,
            IsCapital = false
        };

        var response = await _client.PostAsJsonAsync("/api/cities", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<CityDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal("Kobe Test", created.Name);
        Assert.Equal(1, created.CountryId);
        Assert.Equal("Japan", created.CountryName);
        Assert.Equal("JPN", created.CountryIsoCode);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task CreateCity_InvalidCountryId_ReturnsNotFound()
    {
        var request = new CreateCityRequest
        {
            Name = "Atlantis City",
            CountryId = 99999,
            Population = 50000,
            IsCapital = false
        };

        var response = await _client.PostAsJsonAsync("/api/cities", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCity_DuplicateNameInSameCountry_ReturnsConflict()
    {
        var request = new CreateCityRequest
        {
            Name = "Tokyo", // Already exists in Country 1 (Japan)
            CountryId = 1,
            Population = 1000000,
            IsCapital = false
        };

        var response = await _client.PostAsJsonAsync("/api/cities", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCity_ValidRequest_UpdatesAndReturnsCity()
    {
        // First create a city
        var createRequest = new CreateCityRequest
        {
            Name = "Nagoya Test",
            CountryId = 1,
            StateOrProvince = "Aichi",
            Population = 2300000,
            IsCapital = false
        };
        var createRes = await _client.PostAsJsonAsync("/api/cities", createRequest);
        var created = await createRes.Content.ReadFromJsonAsync<CityDto>(JsonOptions);
        Assert.NotNull(created);

        // Update it
        var updateRequest = new UpdateCityRequest
        {
            Name = "Greater Nagoya",
            CountryId = 1,
            StateOrProvince = "Aichi Prefecture",
            Population = 2350000,
            IsCapital = false
        };

        var updateRes = await _client.PutAsJsonAsync($"/api/cities/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);

        var updated = await updateRes.Content.ReadFromJsonAsync<CityDto>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Greater Nagoya", updated.Name);
        Assert.Equal("Aichi Prefecture", updated.StateOrProvince);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateCity_NonExistent_ReturnsNotFound()
    {
        var updateRequest = new UpdateCityRequest
        {
            Name = "Non Existent City",
            CountryId = 1,
            Population = 1000,
            IsCapital = false
        };

        var response = await _client.PutAsJsonAsync("/api/cities/88888", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCity_ExistingId_DeletesCity()
    {
        // Create city
        var createRequest = new CreateCityRequest
        {
            Name = "City To Remove",
            CountryId = 1,
            Population = 10000,
            IsCapital = false
        };
        var createRes = await _client.PostAsJsonAsync("/api/cities", createRequest);
        var created = await createRes.Content.ReadFromJsonAsync<CityDto>(JsonOptions);
        Assert.NotNull(created);

        // Delete city
        var deleteRes = await _client.DeleteAsync($"/api/cities/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);

        // Verify it is gone
        var getRes = await _client.GetAsync($"/api/cities/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }
}
