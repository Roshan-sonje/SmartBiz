using SmartBiz.Desktop.Models.Brands;
using SmartBiz.Desktop.Models.Common;

namespace SmartBiz.Desktop.Services;

public interface IBrandService
{
    Task<PagedResult<BrandListItem>> GetBrandsAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default);

    Task<Brand> CreateBrandAsync(CreateBrandRequest request, CancellationToken ct = default);
    Task DeleteBrandAsync(Guid id, CancellationToken ct = default);
}