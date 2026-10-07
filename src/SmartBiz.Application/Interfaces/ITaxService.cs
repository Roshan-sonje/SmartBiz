using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Taxes;

namespace SmartBiz.Application.Interfaces;

public interface ITaxService
{
    Task<PagedResult<TaxListItemDto>> GetTaxesAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default);

    Task<TaxDto> GetTaxAsync(Guid businessId, Guid taxId, CancellationToken ct = default);
    Task<TaxDto> CreateTaxAsync(Guid businessId, CreateTaxRequest request, CancellationToken ct = default);
    Task<TaxDto> UpdateTaxAsync(Guid businessId, Guid taxId, UpdateTaxRequest request, CancellationToken ct = default);
    Task DeleteTaxAsync(Guid businessId, Guid taxId, CancellationToken ct = default);
}