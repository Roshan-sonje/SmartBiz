namespace SmartBiz.Desktop.Models.Inventory;

public class InventoryValuation
{
    public int ProductCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalPurchaseValue { get; set; }
    public decimal TotalSellingValue { get; set; }
    public decimal TotalPotentialProfit { get; set; }
}