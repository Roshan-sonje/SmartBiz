using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public Guid? ReplacedByTokenId { get; set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    // Navigation
    public User User { get; set; } = null!;
}