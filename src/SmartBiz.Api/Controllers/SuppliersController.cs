using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Suppliers;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/suppliers")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly ICurrentUser _currentUser;

    public SuppliersController(ISupplierService supplierService, ICurrentUser currentUser)
    {
        _supplierService = supplierService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SupplierListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        var result = await _supplierService.GetSuppliersAsync(
            businessId, page, pageSize, search, includeInactive, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplier(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var supplier = await _supplierService.GetSupplierAsync(businessId, id, ct);
        return Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSupplier(
        [FromBody] CreateSupplierRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var supplier = await _supplierService.CreateSupplierAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, supplier);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSupplier(
        Guid id,
        [FromBody] UpdateSupplierRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var supplier = await _supplierService.UpdateSupplierAsync(businessId, id, request, ct);
        return Ok(supplier);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSupplier(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _supplierService.DeleteSupplierAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
    {
        return _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");
    }
}