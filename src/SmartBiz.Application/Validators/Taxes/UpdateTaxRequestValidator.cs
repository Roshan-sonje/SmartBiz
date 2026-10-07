using FluentValidation;
using SmartBiz.Application.DTOs.Taxes;

namespace SmartBiz.Application.Validators.Taxes;

public class UpdateTaxRequestValidator : AbstractValidator<UpdateTaxRequest>
{
    public UpdateTaxRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}