using FluentValidation;
using SmartBiz.Application.DTOs.Taxes;

namespace SmartBiz.Application.Validators.Taxes;

public class CreateTaxRequestValidator : AbstractValidator<CreateTaxRequest>
{
    public CreateTaxRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tax name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.")
            .LessThanOrEqualTo(100).WithMessage("Tax rate cannot exceed 100.");

        RuleFor(x => x.Description).MaximumLength(500);
    }
}