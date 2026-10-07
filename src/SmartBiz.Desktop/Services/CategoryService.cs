using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Categories;
using SmartBiz.Desktop.Models.Common;

namespace SmartBiz.Desktop.Services;

public class CategoryService : ICategoryService
{
    private readonly IApiClient _api;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IApiClient api, ILogger<CategoryService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<CategoryListItem>> GetCategoriesAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<CategoryListItem>>(
            $"/api/{AppConfig.ApiVersion}/categories{query}", ct);

        return result ?? new PagedResult<CategoryListItem>();
    }

    public async Task<Category> GetCategoryAsync(Guid id, CancellationToken ct = default)
    {
        return await _api.GetAsync<Category>(
            $"/api/{AppConfig.ApiVersion}/categories/{id}", ct)
            ?? throw new InvalidOperationException("Category not found.");
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateCategoryRequest, Category>(
            $"/api/{AppConfig.ApiVersion}/categories", request, ct);
        return result ?? throw new InvalidOperationException("Failed to create category.");
    }

    public async Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateCategoryRequest, Category>(
            $"/api/{AppConfig.ApiVersion}/categories/{id}", request, ct);
        return result ?? throw new InvalidOperationException("Failed to update category.");
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/categories/{id}", ct);
    }
}