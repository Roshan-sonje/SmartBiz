using FluentValidation;
using SmartBiz.Application.DTOs.Business;

namespace SmartBiz.Application.Validators.Business;

public class UpdateBusinessSettingsRequestValidator : AbstractValidator<UpdateBusinessSettingsRequest>
{
    public UpdateBusinessSettingsRequestValidator()
    {
        RuleFor(x => x.InvoicePrefix)
            .NotEmpty().WithMessage("Invoice prefix is required.")
            .MaximumLength(20);

        RuleFor(x => x.InvoiceNumberFormat)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.CurrencySymbol)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.TermsAndConditions).MaximumLength(2000);
        RuleFor(x => x.InvoiceFooterNote).MaximumLength(500);
    }
}