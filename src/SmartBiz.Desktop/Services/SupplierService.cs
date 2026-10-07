using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Suppliers;

namespace SmartBiz.Desktop.Services;

public class SupplierService : ISupplierService
{
    private readonly IApiClient _api;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(IApiClient api, ILogger<SupplierService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<SupplierListItem>> GetSuppliersAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        bool includeInactive = false,
        CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<SupplierListItem>>(
            $"/api/{AppConfig.ApiVersion}/suppliers{query}", ct);

        return result ?? new PagedResult<SupplierListItem>();
    }

    public async Task<Supplier> GetSupplierAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _api.GetAsync<Supplier>(
            $"/api/{AppConfig.ApiVersion}/suppliers/{id}", ct);

        return result ?? throw new InvalidOperationException("Supplier not found.");
    }

    public async Task<Supplier> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateSupplierRequest, Supplier>(
            $"/api/{AppConfig.ApiVersion}/suppliers", request, ct);

        _logger.LogInformation("Supplier created: {Name}", request.Name);
        return result ?? throw new InvalidOperationException("Failed to create supplier.");
    }

    public async Task<Supplier> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request, CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateSupplierRequest, Supplier>(
            $"/api/{AppConfig.ApiVersion}/suppliers/{id}", request, ct);

        _logger.LogInformation("Supplier updated: {Id}", id);
        return result ?? throw new InvalidOperationException("Failed to update supplier.");
    }

    public async Task DeleteSupplierAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/suppliers/{id}", ct);
        _logger.LogInformation("Supplier deleted: {Id}", id);
    }
}