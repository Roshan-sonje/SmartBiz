using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Inventory;

namespace SmartBiz.Desktop.Services;

public class InventoryService : IInventoryService
{
    private readonly IApiClient _api;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IApiClient api, ILogger<InventoryService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<InventoryTransaction> AdjustStockAsync(AdjustStockRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<AdjustStockRequest, InventoryTransaction>(
            $"/api/{AppConfig.ApiVersion}/inventory/adjust", request, ct);

        _logger.LogInformation("Stock adjusted for product {ProductId}: {Adjustment}",
            request.ProductId, request.Adjustment);

        return result ?? throw new InvalidOperationException("Failed to adjust stock.");
    }
    public async Task<List<LowStockProduct>> GetLowStockProductsAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<List<LowStockProduct>>(
            $"/api/{AppConfig.ApiVersion}/inventory/low-stock", ct);

        return result ?? new List<LowStockProduct>();
    }

    public async Task<PagedResult<InventoryTransaction>> GetTransactionsAsync(
        int page = 1, int pageSize = 20, Guid? productId = null, CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}";
        if (productId.HasValue) q += $"&productId={productId.Value}";

        var result = await _api.GetAsync<PagedResult<InventoryTransaction>>(
            $"/api/{AppConfig.ApiVersion}/inventory/transactions{q}", ct);

        return result ?? new PagedResult<InventoryTransaction>();
    }

    public async Task<PagedResult<InventoryTransaction>> GetProductHistoryAsync(
        Guid productId, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var result = await _api.GetAsync<PagedResult<InventoryTransaction>>(
            $"/api/{AppConfig.ApiVersion}/inventory/transactions/{productId}?page={page}&pageSize={pageSize}", ct);

        return result ?? new PagedResult<InventoryTransaction>();
    }

    public async Task<InventoryValuation> GetValuationAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<InventoryValuation>(
            $"/api/{AppConfig.ApiVersion}/inventory/valuation", ct);

        return result ?? new InventoryValuation();
    }
}