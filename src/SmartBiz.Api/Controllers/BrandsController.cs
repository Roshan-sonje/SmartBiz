using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Brands;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/brands")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;
    private readonly ICurrentUser _currentUser;

    public BrandsController(IBrandService brandService, ICurrentUser currentUser)
    {
        _brandService = brandService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BrandListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        var result = await _brandService.GetBrandsAsync(
            businessId, page, pageSize, search, includeInactive, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBrand(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var brand = await _brandService.GetBrandAsync(businessId, id, ct);
        return Ok(brand);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBrand(
        [FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var brand = await _brandService.CreateBrandAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, brand);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBrand(
        Guid id, [FromBody] UpdateBrandRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var brand = await _brandService.UpdateBrandAsync(businessId, id, request, ct);
        return Ok(brand);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBrand(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _brandService.DeleteBrandAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
    {
        return _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");
    }
}