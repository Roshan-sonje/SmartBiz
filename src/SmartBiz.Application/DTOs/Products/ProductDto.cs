namespace SmartBiz.Application.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }

    public Guid UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitShortName { get; set; }

    public Guid? TaxId { get; set; }
    public string? TaxName { get; set; }
    public decimal? TaxRate { get; set; }

    public Guid? PreferredSupplierId { get; set; }
    public string? PreferredSupplierName { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal MinStockLevel { get; set; }

    public bool IsActive { get; set; }
    public bool TrackInventory { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}