using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class Purchase : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public Guid SupplierId { get; set; }

    public string? ReferenceNumber { get; set; }     // supplier's bill no.
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RoundOff { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    // Status
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}