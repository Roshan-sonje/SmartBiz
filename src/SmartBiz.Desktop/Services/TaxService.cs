using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Taxes;

namespace SmartBiz.Desktop.Services;

public class TaxService : ITaxService
{
    private readonly IApiClient _api;
    private readonly ILogger<TaxService> _logger;

    public TaxService(IApiClient api, ILogger<TaxService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<TaxListItem>> GetTaxesAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<TaxListItem>>(
            $"/api/{AppConfig.ApiVersion}/taxes{query}", ct);

        return result ?? new PagedResult<TaxListItem>();
    }

    public async Task<Tax> CreateTaxAsync(CreateTaxRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateTaxRequest, Tax>(
            $"/api/{AppConfig.ApiVersion}/taxes", request, ct);
        return result ?? throw new InvalidOperationException("Failed to create tax.");
    }

    public async Task DeleteTaxAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/taxes/{id}", ct);
        _logger.LogInformation("Tax deleted: {Id}", id);
    }
}