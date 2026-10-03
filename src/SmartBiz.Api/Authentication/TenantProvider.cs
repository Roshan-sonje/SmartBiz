using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Api.Authentication;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid? BusinessId
    {
        get
        {
            var value = Principal?.Claims
                .FirstOrDefault(c => c.Type == "businessId")?.Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? UserId
    {
        get
        {
            var value = Principal?.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                        ?? Principal?.Claims
                            .FirstOrDefault(c => c.Type == "sub")?.Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool HasTenant => BusinessId.HasValue;
}