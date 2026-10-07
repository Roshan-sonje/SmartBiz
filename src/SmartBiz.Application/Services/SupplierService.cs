using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Suppliers;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(IApplicationDbContext db, ILogger<SupplierService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<SupplierListItemDto>> GetSuppliersAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Suppliers
            .IgnoreQueryFilters()
            .Where(s => s.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var str = search.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.Name.ToLower().Contains(str) ||
                (s.Phone != null && s.Phone.Contains(str)) ||
                (s.Email != null && s.Email.ToLower().Contains(str)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SupplierListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Phone = s.Phone,
                Email = s.Email,
                City = s.City,
                OpeningBalance = s.OpeningBalance,
                IsActive = s.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<SupplierListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<SupplierDto> GetSupplierAsync(
        Guid businessId,
        Guid supplierId,
        CancellationToken ct = default)
    {
        var supplier = await _db.Suppliers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.BusinessId == businessId, ct);

        if (supplier is null)
            throw new AuthException("Supplier not found.", 404);

        return MapToDto(supplier);
    }

    public async Task<SupplierDto> CreateSupplierAsync(
        Guid businessId,
        CreateSupplierRequest request,
        CancellationToken ct = default)
    {
        var supplier = new Supplier
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Phone = NullIfEmpty(request.Phone),
            Email = NullIfEmpty(request.Email)?.ToLowerInvariant(),
            Address = NullIfEmpty(request.Address),
            City = NullIfEmpty(request.City),
            State = NullIfEmpty(request.State),
            PostalCode = NullIfEmpty(request.PostalCode),
            Gstin = NullIfEmpty(request.Gstin)?.ToUpperInvariant(),
            OpeningBalance = request.OpeningBalance,
            Notes = NullIfEmpty(request.Notes),
            IsActive = true
        };

        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Supplier created: {SupplierId} ({Name}) for business {BusinessId}",
            supplier.Id, supplier.Name, businessId);

        return MapToDto(supplier);
    }

    public async Task<SupplierDto> UpdateSupplierAsync(
        Guid businessId,
        Guid supplierId,
        UpdateSupplierRequest request,
        CancellationToken ct = default)
    {
        var supplier = await _db.Suppliers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.BusinessId == businessId, ct);

        if (supplier is null)
            throw new AuthException("Supplier not found.", 404);

        supplier.Name = request.Name.Trim();
        supplier.Phone = NullIfEmpty(request.Phone);
        supplier.Email = NullIfEmpty(request.Email)?.ToLowerInvariant();
        supplier.Address = NullIfEmpty(request.Address);
        supplier.City = NullIfEmpty(request.City);
        supplier.State = NullIfEmpty(request.State);
        supplier.PostalCode = NullIfEmpty(request.PostalCode);
        supplier.Gstin = NullIfEmpty(request.Gstin)?.ToUpperInvariant();
        supplier.OpeningBalance = request.OpeningBalance;
        supplier.Notes = NullIfEmpty(request.Notes);
        supplier.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Supplier updated: {SupplierId}", supplierId);

        return MapToDto(supplier);
    }

    public async Task DeleteSupplierAsync(
        Guid businessId,
        Guid supplierId,
        CancellationToken ct = default)
    {
        var supplier = await _db.Suppliers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.BusinessId == businessId, ct);

        if (supplier is null)
            throw new AuthException("Supplier not found.", 404);

        supplier.IsActive = false;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Supplier soft-deleted: {SupplierId}", supplierId);
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SupplierDto MapToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        City = s.City,
        State = s.State,
        PostalCode = s.PostalCode,
        Gstin = s.Gstin,
        OpeningBalance = s.OpeningBalance,
        Notes = s.Notes,
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}