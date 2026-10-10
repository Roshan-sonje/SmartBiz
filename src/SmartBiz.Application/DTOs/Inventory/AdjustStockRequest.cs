namespace SmartBiz.Application.DTOs.Inventory;

public class AdjustStockRequest
{
    public Guid ProductId { get; set; }
    public decimal Adjustment { get; set; }
    public string Reason { get; set; } = string.Empty;
}