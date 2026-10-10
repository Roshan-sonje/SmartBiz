using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Inventory;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    private readonly ICurrentUser _currentUser;

    public InventoryController(IInventoryService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(PagedResult<InventoryTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetTransactionsAsync(businessId, page, pageSize, productId, ct));
    }

    [HttpGet("transactions/{productId:guid}")]
    [ProducesResponseType(typeof(PagedResult<InventoryTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductHistory(
        Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetProductHistoryAsync(businessId, productId, page, pageSize, ct));
    }

    [HttpPost("adjust")]
    [ServiceFilter(typeof(ValidationFilter))]
    [ProducesResponseType(typeof(InventoryTransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdjustStock(
        [FromBody] AdjustStockRequest request,
        CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("User context missing.");

        var result = await _service.AdjustStockAsync(businessId, userId, request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("valuation")]
    [ProducesResponseType(typeof(InventoryValuationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetValuation(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetValuationAsync(businessId, ct));
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(List<LowStockProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetLowStockProductsAsync(businessId, ct));
    }

    private Guid GetBusinessIdOrThrow()
        => _currentUser.BusinessId ?? throw new UnauthorizedAccessException("Business context missing.");
}