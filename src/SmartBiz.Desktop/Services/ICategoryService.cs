using SmartBiz.Desktop.Models.Categories;
using SmartBiz.Desktop.Models.Common;

namespace SmartBiz.Desktop.Services;

public interface ICategoryService
{
    Task<PagedResult<CategoryListItem>> GetCategoriesAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default);

    Task<Category> GetCategoryAsync(Guid id, CancellationToken ct = default);
    Task<Category> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<Category> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken ct = default);
}