using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class AuditLog : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public Guid? UserId { get; set; }

    /// <summary>
    /// What happened — e.g. "CustomerCreated", "SaleCompleted", "PaymentRecorded"
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// The entity type affected — e.g. "Customer", "Sale", "Product"
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// The primary key of the affected record.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// JSON snapshot before the change (optional).
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// JSON snapshot after the change (optional).
    /// </summary>
    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? AdditionalData { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public User? User { get; set; }
}