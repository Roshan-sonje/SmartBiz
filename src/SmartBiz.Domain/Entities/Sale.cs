using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;
using System.Net.ServerSentEvents;

namespace SmartBiz.Domain.Entities;

public class Sale : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public Guid? CustomerId { get; set; }   // null for walk-in customers

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RoundOff { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    // Status
    public SaleStatus Status { get; set; } = SaleStatus.Draft;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Customer? Customer { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public Invoice? Invoice { get; set; }
}