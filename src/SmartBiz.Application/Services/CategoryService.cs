using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Categories;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IApplicationDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<CategoryListItemDto>> GetCategoriesAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Categories
            .IgnoreQueryFilters()
            .Where(c => c.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(c => c.Name.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                IsActive = c.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<CategoryListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<CategoryDto> GetCategoryAsync(
        Guid businessId, Guid categoryId, CancellationToken ct = default)
    {
        var category = await _db.Categories
            .IgnoreQueryFilters()
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.BusinessId == businessId, ct);

        if (category is null)
            throw new AuthException("Category not found.", 404);

        return MapToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(
        Guid businessId, CreateCategoryRequest request, CancellationToken ct = default)
    {
        // Validate parent (if specified)
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _db.Categories
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value
                               && c.BusinessId == businessId, ct);

            if (!parentExists)
                throw new AuthException("Parent category not found.", 400);
        }

        var category = new Category
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Description = NullIfEmpty(request.Description),
            ParentCategoryId = request.ParentCategoryId,
            IsActive = true
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Category created: {CategoryId} ({Name})", category.Id, category.Name);

        return await GetCategoryAsync(businessId, category.Id, ct);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(
        Guid businessId, Guid categoryId, UpdateCategoryRequest request,
        CancellationToken ct = default)
    {
        var category = await _db.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.BusinessId == businessId, ct);

        if (category is null)
            throw new AuthException("Category not found.", 404);

        // Prevent self-parenting
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value == categoryId)
            throw new AuthException("Category cannot be its own parent.", 400);

        // Validate parent
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _db.Categories
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value
                               && c.BusinessId == businessId, ct);

            if (!parentExists)
                throw new AuthException("Parent category not found.", 400);
        }

        category.Name = request.Name.Trim();
        category.Description = NullIfEmpty(request.Description);
        category.ParentCategoryId = request.ParentCategoryId;
        category.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Category updated: {CategoryId}", categoryId);

        return await GetCategoryAsync(businessId, categoryId, ct);
    }

    public async Task DeleteCategoryAsync(Guid businessId, Guid categoryId, CancellationToken ct = default)
    {
        var category = await _db.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.BusinessId == businessId, ct);

        if (category is null)
            throw new AuthException("Category not found.", 404);

        category.IsActive = false;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Category soft-deleted: {CategoryId}", categoryId);
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CategoryDto MapToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        ParentCategoryId = c.ParentCategoryId,
        ParentCategoryName = c.ParentCategory?.Name,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}