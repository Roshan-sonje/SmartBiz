using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class StockAdjustment : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }
    public Guid ProductId { get; set; }

    public decimal PreviousQuantity { get; set; }
    public decimal NewQuantity { get; set; }
    public decimal Difference { get; set; }    // New - Previous

    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public User? CreatedByUser { get; set; }
}