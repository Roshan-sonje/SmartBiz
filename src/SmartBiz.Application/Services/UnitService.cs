using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Units;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class UnitService : IUnitService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<UnitService> _logger;

    public UnitService(IApplicationDbContext db, ILogger<UnitService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<UnitListItemDto>> GetUnitsAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Units
            .IgnoreQueryFilters()
            .Where(u => u.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(u => u.Name.ToLower().Contains(s)
                                     || u.ShortName.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UnitListItemDto
            {
                Id = u.Id,
                Name = u.Name,
                ShortName = u.ShortName,
                AllowDecimal = u.AllowDecimal,
                IsActive = u.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<UnitListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<UnitDto> GetUnitAsync(Guid businessId, Guid unitId, CancellationToken ct = default)
    {
        var unit = await _db.Units
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == unitId && u.BusinessId == businessId, ct);

        if (unit is null) throw new AuthException("Unit not found.", 404);
        return MapToDto(unit);
    }

    public async Task<UnitDto> CreateUnitAsync(Guid businessId, CreateUnitRequest request, CancellationToken ct = default)
    {
        var shortName = request.ShortName.Trim();

        // Unique short name per business
        var exists = await _db.Units
            .IgnoreQueryFilters()
            .AnyAsync(u => u.BusinessId == businessId && u.ShortName == shortName, ct);

        if (exists)
            throw new AuthException($"A unit with short name '{shortName}' already exists.", 409);

        var unit = new Unit
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            ShortName = shortName,
            AllowDecimal = request.AllowDecimal,
            IsActive = true
        };

        _db.Units.Add(unit);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Unit created: {Id} ({Name}/{Short})", unit.Id, unit.Name, unit.ShortName);

        return MapToDto(unit);
    }

    public async Task<UnitDto> UpdateUnitAsync(Guid businessId, Guid unitId, UpdateUnitRequest request, CancellationToken ct = default)
    {
        var unit = await _db.Units
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == unitId && u.BusinessId == businessId, ct);

        if (unit is null) throw new AuthException("Unit not found.", 404);

        var shortName = request.ShortName.Trim();

        // Check uniqueness if short name changed
        if (shortName != unit.ShortName)
        {
            var dup = await _db.Units
                .IgnoreQueryFilters()
                .AnyAsync(u => u.BusinessId == businessId
                               && u.Id != unitId
                               && u.ShortName == shortName, ct);
            if (dup)
                throw new AuthException($"A unit with short name '{shortName}' already exists.", 409);
        }

        unit.Name = request.Name.Trim();
        unit.ShortName = shortName;
        unit.AllowDecimal = request.AllowDecimal;
        unit.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Unit updated: {Id}", unitId);
        return MapToDto(unit);
    }

    public async Task DeleteUnitAsync(Guid businessId, Guid unitId, CancellationToken ct = default)
    {
        var unit = await _db.Units
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == unitId && u.BusinessId == businessId, ct);

        if (unit is null) throw new AuthException("Unit not found.", 404);

        unit.IsActive = false;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Unit soft-deleted: {Id}", unitId);
    }

    private static UnitDto MapToDto(Unit u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        ShortName = u.ShortName,
        AllowDecimal = u.AllowDecimal,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}