using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Api.Filters;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Categories;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
[Authorize]
[ServiceFilter(typeof(ValidationFilter))]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUser _currentUser;

    public CategoriesController(ICategoryService categoryService, ICurrentUser currentUser)
    {
        _categoryService = categoryService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var businessId = GetBusinessIdOrThrow();
        var result = await _categoryService.GetCategoriesAsync(
            businessId, page, pageSize, search, includeInactive, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var category = await _categoryService.GetCategoryAsync(businessId, id, ct);
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var category = await _categoryService.CreateCategoryAsync(businessId, request, ct);
        return StatusCode(StatusCodes.Status201Created, category);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCategory(
        Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        var category = await _categoryService.UpdateCategoryAsync(businessId, id, request, ct);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        var businessId = GetBusinessIdOrThrow();
        await _categoryService.DeleteCategoryAsync(businessId, id, ct);
        return NoContent();
    }

    private Guid GetBusinessIdOrThrow()
    {
        return _currentUser.BusinessId
            ?? throw new UnauthorizedAccessException("Business context is missing from token.");
    }
}