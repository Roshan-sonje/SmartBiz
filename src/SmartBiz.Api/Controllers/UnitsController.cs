using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Units;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/units")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _service;
    private readonly ICurrentUser _currentUser;

    public UnitsController(IUnitService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UnitListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnits(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetUnitsAsync(businessId, page, pageSize, search, includeInactive, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnit(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetUnitAsync(businessId, id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var unit = await _service.CreateUnitAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, unit);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UpdateUnitRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.UpdateUnitAsync(businessId, id, request, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUnit(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _service.DeleteUnitAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
        => _currentUser.BusinessId ?? throw new UnauthorizedAccessException("Business context missing.");
}