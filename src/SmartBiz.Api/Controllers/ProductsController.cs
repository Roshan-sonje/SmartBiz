using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Products;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ICurrentUser _currentUser;

    public ProductsController(IProductService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool lowStockOnly = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        var result = await _service.GetProductsAsync(
            businessId, page, pageSize, search, categoryId, brandId, includeInactive, lowStockOnly, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.GetProductAsync(businessId, id, ct));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var product = await _service.CreateProductAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, product);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        return Ok(await _service.UpdateProductAsync(businessId, id, request, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _service.DeleteProductAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
        => _currentUser.BusinessId ?? throw new UnauthorizedAccessException("Business context missing.");
}