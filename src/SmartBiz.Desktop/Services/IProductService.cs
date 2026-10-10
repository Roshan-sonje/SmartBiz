using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Products;

namespace SmartBiz.Desktop.Services;

public interface IProductService
{
    Task<PagedResult<ProductListItem>> GetProductsAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        Guid? categoryId = null,
        Guid? brandId = null,
        bool includeInactive = false,
        bool lowStockOnly = false,
        CancellationToken ct = default);

    Task<Product> GetProductAsync(Guid id, CancellationToken ct = default);
    Task<Product> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Product> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
}