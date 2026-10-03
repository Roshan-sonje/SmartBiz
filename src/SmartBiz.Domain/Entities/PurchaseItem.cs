using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class PurchaseItem : BaseEntity
{
    public Guid PurchaseId { get; set; }
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;   // snapshot
    public string? ProductSku { get; set; }                    // snapshot

    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    // Navigation
    public Purchase Purchase { get; set; } = null!;
    public Product Product { get; set; } = null!;
}