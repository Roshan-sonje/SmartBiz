using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Taxes;

namespace SmartBiz.Desktop.Services;

public interface ITaxService
{
    Task<PagedResult<TaxListItem>> GetTaxesAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default);

    Task<Tax> CreateTaxAsync(CreateTaxRequest request, CancellationToken ct = default);
    Task DeleteTaxAsync(Guid id, CancellationToken ct = default);
}