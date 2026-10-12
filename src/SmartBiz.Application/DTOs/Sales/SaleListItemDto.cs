namespace SmartBiz.Application.DTOs.Sales;

public class SaleListItemDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = "Walk-in Customer";
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}