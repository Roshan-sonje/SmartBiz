using FluentValidation;
using SmartBiz.Application.DTOs.Business;

namespace SmartBiz.Application.Validators.Business;

public class UpdateBusinessProfileRequestValidator : AbstractValidator<UpdateBusinessProfileRequest>
{
    public UpdateBusinessProfileRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Business name is required.")
            .MaximumLength(200);

        RuleFor(x => x.LegalName).MaximumLength(200);
        RuleFor(x => x.Gstin).MaximumLength(20);
        RuleFor(x => x.Phone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.State).MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(20);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.LogoUrl).MaximumLength(500);
    }
}