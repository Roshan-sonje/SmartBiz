using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Sales;

namespace SmartBiz.Application.Interfaces;

public interface ISaleService
{
    Task<PagedResult<SaleListItemDto>> GetSalesAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        string? paymentStatus,
        Guid? customerId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default);

    Task<SaleDto> GetSaleAsync(Guid businessId, Guid saleId, CancellationToken ct = default);

    Task<SaleDto> CreateSaleAsync(Guid businessId, Guid userId, CreateSaleRequest request, CancellationToken ct = default);

    Task<SaleDto> CancelSaleAsync(Guid businessId, Guid saleId, string reason, CancellationToken ct = default);

    Task<string> GetNextInvoiceNumberAsync(Guid businessId, CancellationToken ct = default);
}