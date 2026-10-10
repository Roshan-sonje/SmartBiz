namespace SmartBiz.Application.DTOs.Inventory;

public class InventoryValuationDto
{
    public int ProductCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalPurchaseValue { get; set; }
    public decimal TotalSellingValue { get; set; }
    public decimal TotalPotentialProfit { get; set; }
}