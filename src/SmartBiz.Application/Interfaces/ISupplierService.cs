using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Suppliers;

namespace SmartBiz.Application.Interfaces;

public interface ISupplierService
{
    Task<PagedResult<SupplierListItemDto>> GetSuppliersAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        bool includeInactive,
        CancellationToken ct = default);

    Task<SupplierDto> GetSupplierAsync(
        Guid businessId,
        Guid supplierId,
        CancellationToken ct = default);

    Task<SupplierDto> CreateSupplierAsync(
        Guid businessId,
        CreateSupplierRequest request,
        CancellationToken ct = default);

    Task<SupplierDto> UpdateSupplierAsync(
        Guid businessId,
        Guid supplierId,
        UpdateSupplierRequest request,
        CancellationToken ct = default);

    Task DeleteSupplierAsync(
        Guid businessId,
        Guid supplierId,
        CancellationToken ct = default);
}