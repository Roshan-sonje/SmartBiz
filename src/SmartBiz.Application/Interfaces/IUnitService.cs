using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Units;

namespace SmartBiz.Application.Interfaces;

public interface IUnitService
{
    Task<PagedResult<UnitListItemDto>> GetUnitsAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default);

    Task<UnitDto> GetUnitAsync(Guid businessId, Guid unitId, CancellationToken ct = default);
    Task<UnitDto> CreateUnitAsync(Guid businessId, CreateUnitRequest request, CancellationToken ct = default);
    Task<UnitDto> UpdateUnitAsync(Guid businessId, Guid unitId, UpdateUnitRequest request, CancellationToken ct = default);
    Task DeleteUnitAsync(Guid businessId, Guid unitId, CancellationToken ct = default);
}