using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

/// <summary>
/// Base class for all authenticated SmartBiz controllers.
/// Provides access to CurrentUser and BusinessId automatically.
/// </summary>
[ApiController]
[Authorize]
public abstract class SmartBizController : ControllerBase
{
    protected ICurrentUser CurrentUser =>
        HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

    protected Guid BusinessId =>
        CurrentUser.BusinessId
        ?? throw new UnauthorizedAccessException("Business context is missing from the current token.");

    protected Guid UserId =>
        CurrentUser.UserId
        ?? throw new UnauthorizedAccessException("User context is missing from the current token.");
}