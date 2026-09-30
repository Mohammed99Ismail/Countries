using Countries.Models.DTOs.City;
using FluentValidation;

namespace Countries.Validators;

public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("City name is required.")
            .Length(1, 100).WithMessage("City name must be between 1 and 100 characters.");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Valid Country ID is required.");

        RuleFor(x => x.StateOrProvince)
            .MaximumLength(100).WithMessage("State/Province cannot exceed 100 characters.");

        RuleFor(x => x.Population)
            .GreaterThanOrEqualTo(0).When(x => x.Population.HasValue)
            .WithMessage("Population cannot be negative.");
    }
}

public class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("City name is required.")
            .Length(1, 100).WithMessage("City name must be between 1 and 100 characters.");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Valid Country ID is required.");

        RuleFor(x => x.StateOrProvince)
            .MaximumLength(100).WithMessage("State/Province cannot exceed 100 characters.");

        RuleFor(x => x.Population)
            .GreaterThanOrEqualTo(0).When(x => x.Population.HasValue)
            .WithMessage("Population cannot be negative.");
    }
}
