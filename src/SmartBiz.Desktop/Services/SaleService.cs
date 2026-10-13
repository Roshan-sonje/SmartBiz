using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Sales;

namespace SmartBiz.Desktop.Services;

public class SaleService : ISaleService
{
    private readonly IApiClient _api;
    private readonly ILogger<SaleService> _logger;

    public SaleService(IApiClient api, ILogger<SaleService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<SaleListItem>> GetSalesAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string? paymentStatus = null,
        Guid? customerId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            q += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(paymentStatus))
            q += $"&paymentStatus={paymentStatus}";
        if (customerId.HasValue)
            q += $"&customerId={customerId.Value}";
        if (fromDate.HasValue)
            q += $"&fromDate={fromDate.Value:O}";
        if (toDate.HasValue)
            q += $"&toDate={toDate.Value:O}";

        var result = await _api.GetAsync<PagedResult<SaleListItem>>(
            $"/api/{AppConfig.ApiVersion}/sales{q}", ct);

        return result ?? new PagedResult<SaleListItem>();
    }

    public async Task<Sale> GetSaleAsync(Guid id, CancellationToken ct = default)
    {
        return await _api.GetAsync<Sale>(
            $"/api/{AppConfig.ApiVersion}/sales/{id}", ct)
            ?? throw new InvalidOperationException("Sale not found.");
    }

    public async Task<Sale> CreateSaleAsync(CreateSaleRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateSaleRequest, Sale>(
            $"/api/{AppConfig.ApiVersion}/sales", request, ct);

        _logger.LogInformation("Sale created: {InvoiceNumber}", result?.InvoiceNumber);
        return result ?? throw new InvalidOperationException("Failed to create sale.");
    }

    public async Task<Sale> CancelSaleAsync(Guid id, string reason, CancellationToken ct = default)
    {
        var body = new { reason };
        var result = await _api.PostAsync<object, Sale>(
            $"/api/{AppConfig.ApiVersion}/sales/{id}/cancel", body, ct);

        _logger.LogInformation("Sale cancelled: {Id}", id);
        return result ?? throw new InvalidOperationException("Failed to cancel sale.");
    }

    public async Task<string> GetNextInvoiceNumberAsync(CancellationToken ct = default)
    {
        var response = await _api.GetAsync<InvoiceNumberResponse>(
            $"/api/{AppConfig.ApiVersion}/sales/next-invoice-number", ct);

        return response?.InvoiceNumber ?? "INV-000001";
    }

    private class InvoiceNumberResponse
    {
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}