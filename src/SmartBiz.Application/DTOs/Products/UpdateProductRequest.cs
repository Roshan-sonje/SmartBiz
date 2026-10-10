namespace SmartBiz.Application.DTOs.Products;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid UnitId { get; set; }
    public Guid? TaxId { get; set; }
    public Guid? PreferredSupplierId { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal MinStockLevel { get; set; }

    public bool IsActive { get; set; } = true;
    public bool TrackInventory { get; set; } = true;
}