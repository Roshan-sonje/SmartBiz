using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Role : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}