using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Suppliers;

namespace SmartBiz.Desktop.Services;

public interface ISupplierService
{
    Task<PagedResult<SupplierListItem>> GetSuppliersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        bool includeInactive = false,
        CancellationToken ct = default);

    Task<Supplier> GetSupplierAsync(Guid id, CancellationToken ct = default);

    Task<Supplier> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken ct = default);

    Task<Supplier> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken ct = default);

    Task DeleteSupplierAsync(Guid id, CancellationToken ct = default);
}