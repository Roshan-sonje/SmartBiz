using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.DTOs.Business;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/business")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class BusinessController : ControllerBase
{
    private readonly IBusinessService _businessService;
    private readonly ICurrentUser _currentUser;

    public BusinessController(IBusinessService businessService, ICurrentUser currentUser)
    {
        _businessService = businessService;
        _currentUser = currentUser;
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var profile = await _businessService.GetProfileAsync(businessId, ct);
        return Ok(profile);
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(BusinessProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateBusinessProfileRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var profile = await _businessService.UpdateProfileAsync(businessId, request, ct);
        return Ok(profile);
    }

    [HttpGet("settings")]
    [ProducesResponseType(typeof(BusinessSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var settings = await _businessService.GetSettingsAsync(businessId, ct);
        return Ok(settings);
    }

    [HttpPut("settings")]
    [ProducesResponseType(typeof(BusinessSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSettings(
        [FromBody] UpdateBusinessSettingsRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var settings = await _businessService.UpdateSettingsAsync(businessId, request, ct);
        return Ok(settings);
    }

    private Guid GetBusinessIdOrThrow()
    {
        return _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");
    }
}