using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Categories;

namespace SmartBiz.Application.Interfaces;

public interface ICategoryService
{
    Task<PagedResult<CategoryListItemDto>> GetCategoriesAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        bool includeInactive,
        CancellationToken ct = default);

    Task<CategoryDto> GetCategoryAsync(
        Guid businessId,
        Guid categoryId,
        CancellationToken ct = default);

    Task<CategoryDto> CreateCategoryAsync(
        Guid businessId,
        CreateCategoryRequest request,
        CancellationToken ct = default);

    Task<CategoryDto> UpdateCategoryAsync(
        Guid businessId,
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken ct = default);

    Task DeleteCategoryAsync(
        Guid businessId,
        Guid categoryId,
        CancellationToken ct = default);
}