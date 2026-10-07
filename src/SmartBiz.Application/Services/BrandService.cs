using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Brands;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class BrandService : IBrandService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<BrandService> _logger;

    public BrandService(IApplicationDbContext db, ILogger<BrandService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<BrandListItemDto>> GetBrandsAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Brands
            .IgnoreQueryFilters()
            .Where(b => b.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(b => b.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(b => b.Name.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BrandListItemDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                IsActive = b.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<BrandListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<BrandDto> GetBrandAsync(Guid businessId, Guid brandId, CancellationToken ct = default)
    {
        var brand = await _db.Brands
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == brandId && b.BusinessId == businessId, ct);

        if (brand is null)
            throw new AuthException("Brand not found.", 404);

        return MapToDto(brand);
    }

    public async Task<BrandDto> CreateBrandAsync(Guid businessId, CreateBrandRequest request, CancellationToken ct = default)
    {
        var brand = new Brand
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Description = NullIfEmpty(request.Description),
            IsActive = true
        };

        _db.Brands.Add(brand);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Brand created: {BrandId} ({Name})", brand.Id, brand.Name);

        return MapToDto(brand);
    }

    public async Task<BrandDto> UpdateBrandAsync(Guid businessId, Guid brandId, UpdateBrandRequest request, CancellationToken ct = default)
    {
        var brand = await _db.Brands
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == brandId && b.BusinessId == businessId, ct);

        if (brand is null)
            throw new AuthException("Brand not found.", 404);

        brand.Name = request.Name.Trim();
        brand.Description = NullIfEmpty(request.Description);
        brand.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Brand updated: {BrandId}", brandId);

        return MapToDto(brand);
    }

    public async Task DeleteBrandAsync(Guid businessId, Guid brandId, CancellationToken ct = default)
    {
        var brand = await _db.Brands
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == brandId && b.BusinessId == businessId, ct);

        if (brand is null)
            throw new AuthException("Brand not found.", 404);

        brand.IsActive = false;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Brand soft-deleted: {BrandId}", brandId);
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static BrandDto MapToDto(Brand b) => new()
    {
        Id = b.Id,
        Name = b.Name,
        Description = b.Description,
        IsActive = b.IsActive,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };
}