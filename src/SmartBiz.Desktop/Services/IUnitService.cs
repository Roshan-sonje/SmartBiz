using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Units;

namespace SmartBiz.Desktop.Services;

public interface IUnitService
{
    Task<PagedResult<UnitListItem>> GetUnitsAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default);

    Task<Unit> CreateUnitAsync(CreateUnitRequest request, CancellationToken ct = default);
    Task DeleteUnitAsync(Guid id, CancellationToken ct = default);
}