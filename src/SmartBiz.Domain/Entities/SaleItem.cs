using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;   // snapshot
    public string? ProductSku { get; set; }                    // snapshot

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    // Navigation
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
}