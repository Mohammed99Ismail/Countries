using Countries.Models.DTOs.Country;
using FluentValidation;

namespace Countries.Validators;

public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    public CreateCountryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Country name is required.")
            .Length(2, 100).WithMessage("Country name must be between 2 and 100 characters.");

        RuleFor(x => x.IsoCode)
            .NotEmpty().WithMessage("ISO code is required.")
            .Length(2, 3).WithMessage("ISO code must be 2 or 3 characters (e.g. US, USA, DE, DEU).")
            .Matches("^[A-Za-z]+$").WithMessage("ISO code must contain only letters.");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region/Continent is required.")
            .Length(2, 100).WithMessage("Region must be between 2 and 100 characters.");

        RuleFor(x => x.Population)
            .GreaterThanOrEqualTo(0).When(x => x.Population.HasValue)
            .WithMessage("Population cannot be negative.");

        RuleFor(x => x.PhoneCode)
            .MaximumLength(10).WithMessage("Phone code cannot exceed 10 characters.");

        RuleFor(x => x.Currency)
            .MaximumLength(10).WithMessage("Currency code cannot exceed 10 characters.");
    }
}

public class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
{
    public UpdateCountryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Country name is required.")
            .Length(2, 100).WithMessage("Country name must be between 2 and 100 characters.");

        RuleFor(x => x.IsoCode)
            .NotEmpty().WithMessage("ISO code is required.")
            .Length(2, 3).WithMessage("ISO code must be 2 or 3 characters (e.g. US, USA, DE, DEU).")
            .Matches("^[A-Za-z]+$").WithMessage("ISO code must contain only letters.");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region/Continent is required.")
            .Length(2, 100).WithMessage("Region must be between 2 and 100 characters.");

        RuleFor(x => x.Population)
            .GreaterThanOrEqualTo(0).When(x => x.Population.HasValue)
            .WithMessage("Population cannot be negative.");

        RuleFor(x => x.PhoneCode)
            .MaximumLength(10).WithMessage("Phone code cannot exceed 10 characters.");

        RuleFor(x => x.Currency)
            .MaximumLength(10).WithMessage("Currency code cannot exceed 10 characters.");
    }
}
