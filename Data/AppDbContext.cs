using Countries.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Countries.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.IsoCode).IsRequired().HasMaxLength(3);
            entity.Property(c => c.Region).IsRequired().HasMaxLength(100);
            entity.Property(c => c.PhoneCode).HasMaxLength(10);
            entity.Property(c => c.Currency).HasMaxLength(10);

            entity.HasIndex(c => c.IsoCode).IsUnique();
            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.Region);

            entity.HasMany(c => c.Cities)
                  .WithOne(city => city.Country)
                  .HasForeignKey(city => city.CountryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.StateOrProvince).HasMaxLength(100);

            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.CountryId);
            entity.HasIndex(c => c.IsCapital);
        });
    }
}
