namespace SmartBiz.Desktop.Models.Sales;

public class CreateSaleRequest
{
    public Guid? CustomerId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
    public decimal PaidAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
}

public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxRate { get; set; }
}