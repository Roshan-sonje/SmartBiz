using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Sales;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/sales")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly ICurrentUser _currentUser;

    public SalesController(ISaleService saleService, ICurrentUser currentUser)
    {
        _saleService = saleService;
        _currentUser = currentUser;
    }


    [HttpGet("summary")]
    [ProducesResponseType(typeof(SalesSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _saleService.GetSummaryAsync(businessId, ct));
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SaleListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSales(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        var result = await _saleService.GetSalesAsync(
            businessId, page, pageSize, search, paymentStatus, customerId, fromDate, toDate, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _saleService.GetSaleAsync(businessId, id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var userId = GetUserIdOrThrow();

        var sale = await _saleService.CreateSaleAsync(businessId, userId, request, ct);
        return StatusCode(StatusCodes.Status201Created, sale);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(
        Guid id, [FromBody] CancelSaleRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();

        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest(new { error = "Reason is required." });

        return Ok(await _saleService.CancelSaleAsync(businessId, id, request.Reason.Trim(), ct));
    }

    [HttpGet("next-invoice-number")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNextInvoiceNumber(CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var number = await _saleService.GetNextInvoiceNumberAsync(businessId, ct);
        return Ok(new { invoiceNumber = number });
    }

    private Guid GetBusinessIdOrThrow()
        => _currentUser.BusinessId ?? throw new UnauthorizedAccessException("Business context missing.");

    private Guid GetUserIdOrThrow()
        => _currentUser.UserId ?? throw new UnauthorizedAccessException("User context missing.");
}