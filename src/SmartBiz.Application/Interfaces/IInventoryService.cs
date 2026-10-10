using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Inventory;

namespace SmartBiz.Application.Interfaces;

public interface IInventoryService
{
    Task<PagedResult<InventoryTransactionDto>> GetTransactionsAsync(
        Guid businessId, int page, int pageSize, Guid? productId, CancellationToken ct = default);

    Task<PagedResult<InventoryTransactionDto>> GetProductHistoryAsync(
        Guid businessId, Guid productId, int page, int pageSize, CancellationToken ct = default);

    Task<InventoryTransactionDto> AdjustStockAsync(
        Guid businessId, Guid userId, AdjustStockRequest request, CancellationToken ct = default);

    Task<InventoryValuationDto> GetValuationAsync(Guid businessId, CancellationToken ct = default);

    Task<List<LowStockProductDto>> GetLowStockProductsAsync(Guid businessId, CancellationToken ct = default);
}