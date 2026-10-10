using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Products;

namespace SmartBiz.Desktop.Services;

public class ProductService : IProductService
{
    private readonly IApiClient _api;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IApiClient api, ILogger<ProductService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<ProductListItem>> GetProductsAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        bool includeInactive = false,
        bool lowStockOnly = false,
        CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}&lowStockOnly={lowStockOnly.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search)) q += $"&search={Uri.EscapeDataString(search)}";
        if (categoryId.HasValue) q += $"&categoryId={categoryId.Value}";
        if (brandId.HasValue) q += $"&brandId={brandId.Value}";

        var result = await _api.GetAsync<PagedResult<ProductListItem>>(
            $"/api/{AppConfig.ApiVersion}/products{q}", ct);

        return result ?? new PagedResult<ProductListItem>();
    }

    public async Task<Product> GetProductAsync(Guid id, CancellationToken ct = default)
    {
        return await _api.GetAsync<Product>(
            $"/api/{AppConfig.ApiVersion}/products/{id}", ct)
            ?? throw new InvalidOperationException("Product not found.");
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateProductRequest, Product>(
            $"/api/{AppConfig.ApiVersion}/products", request, ct);
        _logger.LogInformation("Product created: {Name} ({Sku})", request.Name, request.Sku);
        return result ?? throw new InvalidOperationException("Failed to create product.");
    }

    public async Task<Product> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateProductRequest, Product>(
            $"/api/{AppConfig.ApiVersion}/products/{id}", request, ct);
        _logger.LogInformation("Product updated: {Id}", id);
        return result ?? throw new InvalidOperationException("Failed to update product.");
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/products/{id}", ct);
        _logger.LogInformation("Product deleted: {Id}", id);
    }
}