using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Products;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IApplicationDbContext db, ILogger<ProductService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        Guid? categoryId,
        Guid? brandId,
        bool includeInactive,
        bool lowStockOnly,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Products
            .IgnoreQueryFilters()
            .Where(p => p.BusinessId == businessId);

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

        if (lowStockOnly)
            query = query.Where(p => p.TrackInventory && p.StockQuantity <= p.MinStockLevel);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower().Contains(s) ||
                p.Sku.ToLower().Contains(s) ||
                (p.Barcode != null && p.Barcode.Contains(s)));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Barcode = p.Barcode,
                CategoryName = p.Category != null ? p.Category.Name : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                UnitShortName = p.Unit != null ? p.Unit.ShortName : null,
                PurchasePrice = p.PurchasePrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                MinStockLevel = p.MinStockLevel,
                IsLowStock = p.TrackInventory && p.StockQuantity <= p.MinStockLevel,
                IsActive = p.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<ProductListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ProductDto> GetProductAsync(Guid businessId, Guid productId, CancellationToken ct = default)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Unit)
            .Include(p => p.Tax)
            .Include(p => p.PreferredSupplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.BusinessId == businessId, ct);

        if (product is null)
            throw new AuthException("Product not found.", 404);

        return MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(
        Guid businessId, CreateProductRequest request, CancellationToken ct = default)
    {
        await ValidateReferencesAsync(businessId, request.CategoryId, request.BrandId,
            request.UnitId, request.TaxId, request.PreferredSupplierId, ct);

        var sku = request.Sku.Trim();
        var skuExists = await _db.Products
            .IgnoreQueryFilters()
            .AnyAsync(p => p.BusinessId == businessId && p.Sku == sku, ct);
        if (skuExists)
            throw new AuthException($"A product with SKU '{sku}' already exists.", 409);

        var barcode = NullIfEmpty(request.Barcode);
        if (barcode != null)
        {
            var bcExists = await _db.Products
                .IgnoreQueryFilters()
                .AnyAsync(p => p.BusinessId == businessId && p.Barcode == barcode, ct);
            if (bcExists)
                throw new AuthException($"A product with barcode '{barcode}' already exists.", 409);
        }

        var product = new Product
        {
            BusinessId = businessId,
            Name = request.Name.Trim(),
            Sku = sku,
            Barcode = barcode,
            Description = NullIfEmpty(request.Description),
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            UnitId = request.UnitId,
            TaxId = request.TaxId,
            PreferredSupplierId = request.PreferredSupplierId,
            PurchasePrice = request.PurchasePrice,
            SellingPrice = request.SellingPrice,
            StockQuantity = request.StockQuantity,
            MinStockLevel = request.MinStockLevel,
            TrackInventory = request.TrackInventory,
            IsActive = true
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Product created: {Id} ({Name}) SKU={Sku} for business {BusinessId}",
            product.Id, product.Name, product.Sku, businessId);

        return await GetProductAsync(businessId, product.Id, ct);
    }

    public async Task<ProductDto> UpdateProductAsync(
        Guid businessId, Guid productId, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == productId && p.BusinessId == businessId, ct);

        if (product is null)
            throw new AuthException("Product not found.", 404);

        await ValidateReferencesAsync(businessId, request.CategoryId, request.BrandId,
            request.UnitId, request.TaxId, request.PreferredSupplierId, ct);

        var sku = request.Sku.Trim();
        if (sku != product.Sku)
        {
            var skuExists = await _db.Products
                .IgnoreQueryFilters()
                .AnyAsync(p => p.BusinessId == businessId && p.Id != productId && p.Sku == sku, ct);
            if (skuExists)
                throw new AuthException($"A product with SKU '{sku}' already exists.", 409);
        }

        var barcode = NullIfEmpty(request.Barcode);
        if (barcode != null && barcode != product.Barcode)
        {
            var bcExists = await _db.Products
                .IgnoreQueryFilters()
                .AnyAsync(p => p.BusinessId == businessId && p.Id != productId && p.Barcode == barcode, ct);
            if (bcExists)
                throw new AuthException($"A product with barcode '{barcode}' already exists.", 409);
        }

        product.Name = request.Name.Trim();
        product.Sku = sku;
        product.Barcode = barcode;
        product.Description = NullIfEmpty(request.Description);
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.UnitId = request.UnitId;
        product.TaxId = request.TaxId;
        product.PreferredSupplierId = request.PreferredSupplierId;
        product.PurchasePrice = request.PurchasePrice;
        product.SellingPrice = request.SellingPrice;
        product.StockQuantity = request.StockQuantity;
        product.MinStockLevel = request.MinStockLevel;
        product.TrackInventory = request.TrackInventory;
        product.IsActive = request.IsActive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Product updated: {Id}", productId);

        return await GetProductAsync(businessId, productId, ct);
    }

    public async Task DeleteProductAsync(Guid businessId, Guid productId, CancellationToken ct = default)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == productId && p.BusinessId == businessId, ct);

        if (product is null)
            throw new AuthException("Product not found.", 404);

        product.IsActive = false;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Product soft-deleted: {Id}", productId);
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private async Task ValidateReferencesAsync(
        Guid businessId,
        Guid? categoryId,
        Guid? brandId,
        Guid unitId,
        Guid? taxId,
        Guid? preferredSupplierId,
        CancellationToken ct)
    {
        if (categoryId.HasValue)
        {
            var ok = await _db.Categories.IgnoreQueryFilters()
                .AnyAsync(c => c.Id == categoryId.Value && c.BusinessId == businessId, ct);
            if (!ok) throw new AuthException("Category not found.", 400);
        }

        if (brandId.HasValue)
        {
            var ok = await _db.Brands.IgnoreQueryFilters()
                .AnyAsync(b => b.Id == brandId.Value && b.BusinessId == businessId, ct);
            if (!ok) throw new AuthException("Brand not found.", 400);
        }

        var unitOk = await _db.Units.IgnoreQueryFilters()
            .AnyAsync(u => u.Id == unitId && u.BusinessId == businessId, ct);
        if (!unitOk) throw new AuthException("Unit not found.", 400);

        if (taxId.HasValue)
        {
            var ok = await _db.Taxes.IgnoreQueryFilters()
                .AnyAsync(t => t.Id == taxId.Value && t.BusinessId == businessId, ct);
            if (!ok) throw new AuthException("Tax not found.", 400);
        }

        if (preferredSupplierId.HasValue)
        {
            var ok = await _db.Suppliers.IgnoreQueryFilters()
                .AnyAsync(s => s.Id == preferredSupplierId.Value && s.BusinessId == businessId, ct);
            if (!ok) throw new AuthException("Supplier not found.", 400);
        }
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Sku = p.Sku,
        Barcode = p.Barcode,
        Description = p.Description,
        ImageUrl = p.ImageUrl,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        UnitId = p.UnitId,
        UnitName = p.Unit?.Name,
        UnitShortName = p.Unit?.ShortName,
        TaxId = p.TaxId,
        TaxName = p.Tax?.Name,
        TaxRate = p.Tax?.Rate,
        PreferredSupplierId = p.PreferredSupplierId,
        PreferredSupplierName = p.PreferredSupplier?.Name,
        PurchasePrice = p.PurchasePrice,
        SellingPrice = p.SellingPrice,
        StockQuantity = p.StockQuantity,
        MinStockLevel = p.MinStockLevel,
        IsActive = p.IsActive,
        TrackInventory = p.TrackInventory,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}