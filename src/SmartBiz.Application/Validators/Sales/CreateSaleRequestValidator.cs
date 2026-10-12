using FluentValidation;
using SmartBiz.Application.DTOs.Sales;

namespace SmartBiz.Application.Validators.Sales;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Sale must have items.")
            .Must(i => i.Count > 0).WithMessage("Sale must have at least one item.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateSaleItemRequestValidator());

        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.PaymentMethod).MaximumLength(50);
        RuleFor(x => x.PaymentReference).MaximumLength(100);
    }
}