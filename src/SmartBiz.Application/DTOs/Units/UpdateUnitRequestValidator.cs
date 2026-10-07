using FluentValidation;
using SmartBiz.Application.DTOs.Units;

namespace SmartBiz.Application.Validators.Units;

public class UpdateUnitRequestValidator : AbstractValidator<UpdateUnitRequest>
{
    public UpdateUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShortName).NotEmpty().MaximumLength(20);
    }
}