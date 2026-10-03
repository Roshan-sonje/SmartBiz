namespace SmartBiz.Application.Interfaces;

/// <summary>
/// Provides the current tenant (business) context for the request.
/// Used by EF Core global query filters to enforce tenant isolation.
/// </summary>
public interface ITenantProvider
{
    Guid? BusinessId { get; }
    Guid? UserId { get; }
    bool HasTenant { get; }
}