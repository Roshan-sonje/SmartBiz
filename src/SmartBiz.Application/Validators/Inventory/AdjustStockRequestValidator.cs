using FluentValidation;
using SmartBiz.Application.DTOs.Inventory;

namespace SmartBiz.Application.Validators.Inventory;

public class AdjustStockRequestValidator : AbstractValidator<AdjustStockRequest>
{
    public AdjustStockRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.Adjustment)
            .NotEqual(0).WithMessage("Adjustment cannot be zero.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(200);
    }
}