using Countries.Data;
using Countries.Middleware;
using Countries.Services;
using Countries.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context (In-Memory for zero-setup execution)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("CountriesCitiesDb");
});

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateCountryRequestValidator>();

// Register Application Services
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICityService, CityService>();

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Configure OpenAPI & Documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed initial demo data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Global Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// OpenAPI and Scalar Interactive Documentation
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Countries & Cities API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

// Quick redirect from root to UI or Scalar if desired
app.MapGet("/api", () => Results.Ok(new
{
    Message = "Countries & Cities API is running.",
    Documentation = "/scalar/v1",
    OpenApi = "/openapi/v1.json"
}));

app.Run();

// Make Program accessible for WebApplicationFactory in integration tests
public partial class Program { }
