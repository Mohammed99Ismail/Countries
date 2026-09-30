using Countries.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Countries.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Countries.AnyAsync())
        {
            return; // Data already seeded
        }

        var countries = new List<Country>
        {
            new Country
            {
                Name = "Japan",
                IsoCode = "JPN",
                Region = "Asia",
                PhoneCode = "+81",
                Currency = "JPY",
                Population = 125120000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-12),
                Cities = new List<City>
                {
                    new City { Name = "Tokyo", StateOrProvince = "Kanto", Population = 13960000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-12) },
                    new City { Name = "Osaka", StateOrProvince = "Kansai", Population = 2691000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-11) },
                    new City { Name = "Kyoto", StateOrProvince = "Kansai", Population = 1464000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-11) },
                    new City { Name = "Yokohama", StateOrProvince = "Kanagawa", Population = 3770000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-10) }
                }
            },
            new Country
            {
                Name = "Germany",
                IsoCode = "DEU",
                Region = "Europe",
                PhoneCode = "+49",
                Currency = "EUR",
                Population = 84400000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-10),
                Cities = new List<City>
                {
                    new City { Name = "Berlin", StateOrProvince = "Berlin", Population = 3645000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-10) },
                    new City { Name = "Munich", StateOrProvince = "Bavaria", Population = 1472000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-9) },
                    new City { Name = "Hamburg", StateOrProvince = "Hamburg", Population = 1841000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-9) },
                    new City { Name = "Frankfurt", StateOrProvince = "Hesse", Population = 753000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-8) }
                }
            },
            new Country
            {
                Name = "United States",
                IsoCode = "USA",
                Region = "Americas",
                PhoneCode = "+1",
                Currency = "USD",
                Population = 333287557,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-9),
                Cities = new List<City>
                {
                    new City { Name = "Washington, D.C.", StateOrProvince = "District of Columbia", Population = 689545, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-9) },
                    new City { Name = "New York", StateOrProvince = "New York", Population = 8336817, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-8) },
                    new City { Name = "Los Angeles", StateOrProvince = "California", Population = 3822238, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-8) },
                    new City { Name = "Chicago", StateOrProvince = "Illinois", Population = 2665039, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-7) }
                }
            },
            new Country
            {
                Name = "United Kingdom",
                IsoCode = "GBR",
                Region = "Europe",
                PhoneCode = "+44",
                Currency = "GBP",
                Population = 67330000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-8),
                Cities = new List<City>
                {
                    new City { Name = "London", StateOrProvince = "Greater London", Population = 8982000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-8) },
                    new City { Name = "Edinburgh", StateOrProvince = "Scotland", Population = 548000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-7) },
                    new City { Name = "Manchester", StateOrProvince = "Greater Manchester", Population = 553000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-7) }
                }
            },
            new Country
            {
                Name = "Canada",
                IsoCode = "CAN",
                Region = "Americas",
                PhoneCode = "+1",
                Currency = "CAD",
                Population = 38250000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-7),
                Cities = new List<City>
                {
                    new City { Name = "Ottawa", StateOrProvince = "Ontario", Population = 1017000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-7) },
                    new City { Name = "Toronto", StateOrProvince = "Ontario", Population = 2930000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-6) },
                    new City { Name = "Vancouver", StateOrProvince = "British Columbia", Population = 675218, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-6) },
                    new City { Name = "Montreal", StateOrProvince = "Quebec", Population = 1762949, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-5) }
                }
            },
            new Country
            {
                Name = "Australia",
                IsoCode = "AUS",
                Region = "Oceania",
                PhoneCode = "+61",
                Currency = "AUD",
                Population = 25690000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-6),
                Cities = new List<City>
                {
                    new City { Name = "Canberra", StateOrProvince = "Australian Capital Territory", Population = 456652, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-6) },
                    new City { Name = "Sydney", StateOrProvince = "New South Wales", Population = 5312000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-5) },
                    new City { Name = "Melbourne", StateOrProvince = "Victoria", Population = 5078000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-5) }
                }
            },
            new Country
            {
                Name = "France",
                IsoCode = "FRA",
                Region = "Europe",
                PhoneCode = "+33",
                Currency = "EUR",
                Population = 67750000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-5),
                Cities = new List<City>
                {
                    new City { Name = "Paris", StateOrProvince = "Île-de-France", Population = 2161000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-5) },
                    new City { Name = "Marseille", StateOrProvince = "Provence-Alpes-Côte d'Azur", Population = 861635, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-4) },
                    new City { Name = "Lyon", StateOrProvince = "Auvergne-Rhône-Alpes", Population = 513275, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-4) }
                }
            },
            new Country
            {
                Name = "Brazil",
                IsoCode = "BRA",
                Region = "Americas",
                PhoneCode = "+55",
                Currency = "BRL",
                Population = 214300000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-4),
                Cities = new List<City>
                {
                    new City { Name = "Brasília", StateOrProvince = "Federal District", Population = 3055149, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-4) },
                    new City { Name = "São Paulo", StateOrProvince = "São Paulo", Population = 12330000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3) },
                    new City { Name = "Rio de Janeiro", StateOrProvince = "Rio de Janeiro", Population = 6748000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3) }
                }
            },
            new Country
            {
                Name = "Egypt",
                IsoCode = "EGY",
                Region = "Africa",
                PhoneCode = "+20",
                Currency = "EGP",
                Population = 104300000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3),
                Cities = new List<City>
                {
                    new City { Name = "Cairo", StateOrProvince = "Cairo Governorate", Population = 9606916, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3) },
                    new City { Name = "Alexandria", StateOrProvince = "Alexandria Governorate", Population = 5200000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2) },
                    new City { Name = "Giza", StateOrProvince = "Giza Governorate", Population = 4366590, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2) }
                }
            },
            new Country
            {
                Name = "India",
                IsoCode = "IND",
                Region = "Asia",
                PhoneCode = "+91",
                Currency = "INR",
                Population = 1408000000,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
                Cities = new List<City>
                {
                    new City { Name = "New Delhi", StateOrProvince = "Delhi", Population = 32900000, IsCapital = true, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2) },
                    new City { Name = "Mumbai", StateOrProvince = "Maharashtra", Population = 20961000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-1) },
                    new City { Name = "Bengaluru", StateOrProvince = "Karnataka", Population = 13193000, IsCapital = false, CreatedAt = DateTimeOffset.UtcNow.AddMonths(-1) }
                }
            }
        };

        await context.Countries.AddRangeAsync(countries);
        await context.SaveChangesAsync();
    }
}
