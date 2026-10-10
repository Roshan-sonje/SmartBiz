namespace SmartBiz.Application.DTOs.Products;

public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? CategoryName { get; set; }
    public string? BrandName { get; set; }
    public string? UnitShortName { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal MinStockLevel { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsActive { get; set; }
}