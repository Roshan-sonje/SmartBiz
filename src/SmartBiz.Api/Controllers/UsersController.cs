using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.DTOs.Users;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    public UsersController(IUserService userService, ICurrentUser currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var users = await _userService.GetUsersAsync(businessId, ct);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var user = await _userService.GetUserAsync(businessId, id, ct);
        return Ok(user);
    }

    [HttpPost("invite")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Invite(
        [FromBody] InviteUserRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var userId = GetUserIdOrThrow();

        var user = await _userService.InviteUserAsync(businessId, userId, request, ct);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPut("{id:guid}/roles")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoles(
        Guid id,
        [FromBody] UpdateUserRolesRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var user = await _userService.UpdateUserRolesAsync(businessId, id, request, ct);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var currentUserId = GetUserIdOrThrow();

        await _userService.DeactivateUserAsync(businessId, currentUserId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
    {
        return _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");
    }

    private Guid GetUserIdOrThrow()
    {
        return _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User context is missing from token.");
    }
}