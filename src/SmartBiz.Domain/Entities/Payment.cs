using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class Payment : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public PaymentType Type { get; set; }

    // Exactly one of these will be set (customer or supplier)
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }

    // Optional: link to a specific sale / purchase
    public Guid? SaleId { get; set; }
    public Guid? PurchaseId { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    public string? Reference { get; set; }    // cheque no, txn id, etc.
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Customer? Customer { get; set; }
    public Supplier? Supplier { get; set; }
    public Sale? Sale { get; set; }
    public Purchase? Purchase { get; set; }
    public User? CreatedByUser { get; set; }
}