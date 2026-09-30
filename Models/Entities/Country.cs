namespace Countries.Models.Entities;

public class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IsoCode { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string? PhoneCode { get; set; }

    public string? Currency { get; set; }

    public long? Population { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation property
    public List<City> Cities { get; set; } = new();
}
