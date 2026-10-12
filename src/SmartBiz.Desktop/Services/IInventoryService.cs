using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Inventory;

namespace SmartBiz.Desktop.Services;

public interface IInventoryService
{
    Task<InventoryTransaction> AdjustStockAsync(AdjustStockRequest request, CancellationToken ct = default);

    Task<PagedResult<InventoryTransaction>> GetTransactionsAsync(
        int page = 1, int pageSize = 20, Guid? productId = null, CancellationToken ct = default);

    Task<PagedResult<InventoryTransaction>> GetProductHistoryAsync(
        Guid productId, int page = 1, int pageSize = 50, CancellationToken ct = default);

    Task<InventoryValuation> GetValuationAsync(CancellationToken ct = default);
}