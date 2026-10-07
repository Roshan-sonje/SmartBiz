using FluentValidation;
using SmartBiz.Application.DTOs.Brands;

namespace SmartBiz.Application.Validators.Brands;

public class UpdateBrandRequestValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(500);
    }
}