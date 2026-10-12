namespace SmartBiz.Desktop.Models.Inventory;

public class LowStockProduct
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal StockQuantity { get; set; }
    public decimal MinStockLevel { get; set; }
    public string? UnitShortName { get; set; }
    public decimal Deficit { get; set; }
}