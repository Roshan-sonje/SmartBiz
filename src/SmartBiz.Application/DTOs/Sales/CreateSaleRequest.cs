namespace SmartBiz.Application.DTOs.Sales;

public class CreateSaleRequest
{
    public Guid? CustomerId { get; set; }   // null = walk-in
    public DateTime? InvoiceDate { get; set; }
    public string? Notes { get; set; }

    public List<CreateSaleItemRequest> Items { get; set; } = new();

    // Optional immediate payment
    public decimal PaidAmount { get; set; }
    public string? PaymentMethod { get; set; }   // "Cash", "Card", "Upi", etc.
    public string? PaymentReference { get; set; }
}