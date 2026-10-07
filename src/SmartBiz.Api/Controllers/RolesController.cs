using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Application.DTOs.Users;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    public RolesController(IUserService userService, ICurrentUser currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var businessId = _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");

        var roles = await _userService.GetRolesAsync(businessId, ct);
        return Ok(roles);
    }
}