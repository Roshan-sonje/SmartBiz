using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Sales;

namespace SmartBiz.Desktop.Services;

public interface ISaleService
{
    Task<PagedResult<SaleListItem>> GetSalesAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default);

    Task<Sale> GetSaleAsync(Guid id, CancellationToken ct = default);

    Task<Sale> CreateSaleAsync(CreateSaleRequest request, CancellationToken ct = default);

    Task<Sale> CancelSaleAsync(Guid id, string reason, CancellationToken ct = default);

    Task<string> GetNextInvoiceNumberAsync(CancellationToken ct = default);
}