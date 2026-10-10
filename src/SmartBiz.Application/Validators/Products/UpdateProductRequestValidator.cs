using FluentValidation;
using SmartBiz.Application.DTOs.Products;

namespace SmartBiz.Application.Validators.Products;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Barcode).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinStockLevel).GreaterThanOrEqualTo(0);
    }
}