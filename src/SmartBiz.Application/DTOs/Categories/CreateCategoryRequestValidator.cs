using FluentValidation;
using SmartBiz.Application.DTOs.Categories;

namespace SmartBiz.Application.Validators.Categories;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description).MaximumLength(500);
    }
}