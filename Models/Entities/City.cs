namespace Countries.Models.Entities;

public class City
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }

    // Navigation property
    public Country? Country { get; set; }

    public string? StateOrProvince { get; set; }

    public long? Population { get; set; }

    public bool IsCapital { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
