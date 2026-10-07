using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Brands;

namespace SmartBiz.Application.Interfaces;

public interface IBrandService
{
    Task<PagedResult<BrandListItemDto>> GetBrandsAsync(
        Guid businessId, int page, int pageSize, string? search, bool includeInactive,
        CancellationToken ct = default);

    Task<BrandDto> GetBrandAsync(Guid businessId, Guid brandId, CancellationToken ct = default);

    Task<BrandDto> CreateBrandAsync(Guid businessId, CreateBrandRequest request, CancellationToken ct = default);

    Task<BrandDto> UpdateBrandAsync(Guid businessId, Guid brandId, UpdateBrandRequest request, CancellationToken ct = default);

    Task DeleteBrandAsync(Guid businessId, Guid brandId, CancellationToken ct = default);
}