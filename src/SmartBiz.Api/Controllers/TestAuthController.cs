using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Application.Common;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/test-auth")]
[Authorize]
public class TestAuthController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public TestAuthController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet("any-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult AnyUser()
    {
        return Ok(new
        {
            message = "Any authenticated user can access this.",
            email = _currentUser.Email,
            businessId = _currentUser.BusinessId
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message = "Only Admins can access this.",
            roles = _currentUser.Roles
        });
    }
}