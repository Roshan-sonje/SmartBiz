using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Taxes;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class TaxService : ITaxService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<TaxService> _logger;

    public TaxService(IApplicationDbContext db, ILogger<TaxService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<TaxListItemDto>> GetTaxesAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Taxes
            .IgnoreQueryFilters()
            .Where(t => t.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(t => t.Name.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(t => t.Rate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaxListItemDto
            {
                Id = t.Id,
                Name = t.Name,
                Rate = t.Rate,
                Description = t.Description,
                IsActive = t.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<TaxListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<TaxDto> GetTaxAsync(Guid businessId, Guid taxId, CancellationToken ct = default)
    {
        var tax = await _db.Taxes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taxId && t.BusinessId == businessId, ct);

        if (tax is null) throw new AuthException("Tax not found.", 404);
        return MapToDto(tax);
    }

    public async Task<TaxDto> CreateTaxAsync(Guid businessId, CreateTaxRequest request, CancellationToken ct = default)
    {
        var tax = new Tax
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Rate = request.Rate,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = true
        };

        _db.Taxes.Add(tax);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tax created: {Id} ({Name} @ {Rate}%)", tax.Id, tax.Name, tax.Rate);
        return MapToDto(tax);
    }

    public async Task<TaxDto> UpdateTaxAsync(Guid businessId, Guid taxId, UpdateTaxRequest request, CancellationToken ct = default)
    {
        var tax = await _db.Taxes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == taxId && t.BusinessId == businessId, ct);

        if (tax is null) throw new AuthException("Tax not found.", 404);

        tax.Name = request.Name.Trim();
        tax.Rate = request.Rate;
        tax.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        tax.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tax updated: {Id}", taxId);
        return MapToDto(tax);
    }

    public async Task DeleteTaxAsync(Guid businessId, Guid taxId, CancellationToken ct = default)
    {
        var tax = await _db.Taxes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == taxId && t.BusinessId == businessId, ct);

        if (tax is null) throw new AuthException("Tax not found.", 404);

        tax.IsActive = false;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tax soft-deleted: {Id}", taxId);
    }

    private static TaxDto MapToDto(Tax t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Rate = t.Rate,
        Description = t.Description,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };
}