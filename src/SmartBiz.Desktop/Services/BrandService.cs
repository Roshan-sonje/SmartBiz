using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Brands;
using SmartBiz.Desktop.Models.Common;

namespace SmartBiz.Desktop.Services;

public class BrandService : IBrandService
{
    private readonly IApiClient _api;
    private readonly ILogger<BrandService> _logger;

    public BrandService(IApiClient api, ILogger<BrandService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<BrandListItem>> GetBrandsAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<BrandListItem>>(
            $"/api/{AppConfig.ApiVersion}/brands{query}", ct);
        return result ?? new PagedResult<BrandListItem>();
    }

    public async Task<Brand> CreateBrandAsync(CreateBrandRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateBrandRequest, Brand>(
            $"/api/{AppConfig.ApiVersion}/brands", request, ct);
        return result ?? throw new InvalidOperationException("Failed to create brand.");
    }

    public async Task DeleteBrandAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/brands/{id}", ct);
    }
}