using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class User : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}