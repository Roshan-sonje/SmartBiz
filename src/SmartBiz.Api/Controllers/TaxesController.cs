using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Taxes;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/taxes")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class TaxesController : ControllerBase
{
    private readonly ITaxService _service;
    private readonly ICurrentUser _currentUser;

    public TaxesController(ITaxService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaxListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaxes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetTaxesAsync(businessId, page, pageSize, search, includeInactive, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaxDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTax(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetTaxAsync(businessId, id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaxDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTax([FromBody] CreateTaxRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var tax = await _service.CreateTaxAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, tax);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaxDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTax(Guid id, [FromBody] UpdateTaxRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.UpdateTaxAsync(businessId, id, request, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTax(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _service.DeleteTaxAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
        => _currentUser.BusinessId ?? throw new UnauthorizedAccessException("Business context missing.");
}