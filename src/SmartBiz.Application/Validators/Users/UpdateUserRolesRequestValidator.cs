using FluentValidation;
using SmartBiz.Application.DTOs.Users;

namespace SmartBiz.Application.Validators.Users;

public class UpdateUserRolesRequestValidator : AbstractValidator<UpdateUserRolesRequest>
{
    public UpdateUserRolesRequestValidator()
    {
        RuleFor(x => x.RoleIds)
            .NotEmpty().WithMessage("At least one role must be assigned.");
    }
}