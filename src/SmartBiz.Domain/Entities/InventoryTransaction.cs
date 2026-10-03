using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class InventoryTransaction : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }
    public Guid ProductId { get; set; }

    public InventoryTransactionType Type { get; set; }

    /// <summary>
    /// Positive = stock added, Negative = stock removed.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Product.StockQuantity after this transaction — for audit.
    /// </summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// What kind of record caused this (Sale, Purchase, StockAdjustment).
    /// </summary>
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public User? CreatedByUser { get; set; }
}