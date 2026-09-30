using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Countries.Models.DTOs.City;
using Countries.Models.DTOs.Common;
using Countries.Models.DTOs.Country;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Countries.Tests;

public class CountriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CountriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCountries_ReturnsPaginatedList()
    {
        var response = await _client.GetAsync("/api/countries?pageNumber=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CountryDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.Equal(1, paged.PageNumber);
        Assert.Equal(5, paged.PageSize);
        Assert.True(paged.TotalCount >= 10);
        Assert.Equal(5, paged.Items.Count);
    }

    [Fact]
    public async Task GetCountries_WithSearchFilter_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/api/countries?search=Japan");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CountryDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.Single(paged.Items);
        Assert.Equal("Japan", paged.Items[0].Name);
        Assert.Equal("JPN", paged.Items[0].IsoCode);
    }

    [Fact]
    public async Task GetCountries_WithRegionFilter_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/api/countries?region=Europe");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CountryDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.NotEmpty(paged.Items);
        Assert.All(paged.Items, item => Assert.Equal("Europe", item.Region));
    }

    [Fact]
    public async Task GetCountryById_ValidId_ReturnsCountryWithCities()
    {
        var response = await _client.GetAsync("/api/countries/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var country = await response.Content.ReadFromJsonAsync<CountryDetailDto>(JsonOptions);
        Assert.NotNull(country);
        Assert.Equal(1, country.Id);
        Assert.NotEmpty(country.Name);
        Assert.NotEmpty(country.Cities);
    }

    [Fact]
    public async Task GetCountryById_InvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/countries/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCountry_ValidRequest_ReturnsCreatedCountry()
    {
        var request = new CreateCountryRequest
        {
            Name = "New Zealand",
            IsoCode = "NZL",
            Region = "Oceania",
            PhoneCode = "+64",
            Currency = "NZD",
            Population = 5124000
        };

        var response = await _client.PostAsJsonAsync("/api/countries", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<CountryDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal("New Zealand", created.Name);
        Assert.Equal("NZL", created.IsoCode);
        Assert.Equal("Oceania", created.Region);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task CreateCountry_DuplicateIsoCode_ReturnsConflict()
    {
        var request = new CreateCountryRequest
        {
            Name = "Japan Duplicate",
            IsoCode = "JPN", // Already exists
            Region = "Asia",
            Population = 1000000
        };

        var response = await _client.PostAsJsonAsync("/api/countries", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateCountry_InvalidData_ReturnsBadRequest()
    {
        var request = new CreateCountryRequest
        {
            Name = "", // Invalid: empty
            IsoCode = "TOOLONGCODE", // Invalid: > 3 chars
            Region = "",
            Population = -100 // Invalid: negative
        };

        var response = await _client.PostAsJsonAsync("/api/countries", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCountry_ValidRequest_UpdatesAndReturnsCountry()
    {
        // First create a country to update
        var createRequest = new CreateCountryRequest
        {
            Name = "Sweden Test",
            IsoCode = "SWT",
            Region = "Europe",
            Population = 10400000
        };
        var createRes = await _client.PostAsJsonAsync("/api/countries", createRequest);
        var created = await createRes.Content.ReadFromJsonAsync<CountryDto>(JsonOptions);
        Assert.NotNull(created);

        // Update it
        var updateRequest = new UpdateCountryRequest
        {
            Name = "Kingdom of Sweden",
            IsoCode = "SWE",
            Region = "Europe",
            Currency = "SEK",
            PhoneCode = "+46",
            Population = 10500000
        };

        var updateRes = await _client.PutAsJsonAsync($"/api/countries/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateRes.StatusCode);

        var updated = await updateRes.Content.ReadFromJsonAsync<CountryDto>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Kingdom of Sweden", updated.Name);
        Assert.Equal("SWE", updated.IsoCode);
        Assert.Equal("SEK", updated.Currency);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateCountry_NonExistent_ReturnsNotFound()
    {
        var updateRequest = new UpdateCountryRequest
        {
            Name = "Unknown Land",
            IsoCode = "UNK",
            Region = "Unknown",
            Population = 1000
        };

        var response = await _client.PutAsJsonAsync("/api/countries/88888", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCountry_ExistingId_DeletesCountry()
    {
        // Create country
        var createRequest = new CreateCountryRequest
        {
            Name = "Country To Delete",
            IsoCode = "DEL",
            Region = "Europe"
        };
        var createRes = await _client.PostAsJsonAsync("/api/countries", createRequest);
        var created = await createRes.Content.ReadFromJsonAsync<CountryDto>(JsonOptions);
        Assert.NotNull(created);

        // Delete it
        var deleteRes = await _client.DeleteAsync($"/api/countries/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRes.StatusCode);

        // Verify it's gone
        var getRes = await _client.GetAsync($"/api/countries/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task GetCitiesByCountryId_ValidCountry_ReturnsPaginatedCities()
    {
        var response = await _client.GetAsync("/api/countries/1/cities?pageNumber=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CityDto>>(JsonOptions);
        Assert.NotNull(paged);
        Assert.NotEmpty(paged.Items);
        Assert.All(paged.Items, city => Assert.Equal(1, city.CountryId));
    }

    [Fact]
    public async Task GetCitiesByCountryId_InvalidCountry_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/countries/99999/cities");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
