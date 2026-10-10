using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Products;

namespace SmartBiz.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetProductsAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        Guid? categoryId,
        Guid? brandId,
        bool includeInactive,
        bool lowStockOnly,
        CancellationToken ct = default);

    Task<ProductDto> GetProductAsync(Guid businessId, Guid productId, CancellationToken ct = default);

    Task<ProductDto> CreateProductAsync(Guid businessId, CreateProductRequest request, CancellationToken ct = default);

    Task<ProductDto> UpdateProductAsync(Guid businessId, Guid productId, UpdateProductRequest request, CancellationToken ct = default);

    Task DeleteProductAsync(Guid businessId, Guid productId, CancellationToken ct = default);
}