using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartBiz.Application.Interfaces;

namespace SmartBiz.Infrastructure.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

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

    public Guid? BusinessId
    {
        get
        {
            var value = Principal?.Claims
                .FirstOrDefault(c => c.Type == "businessId")?.Value;

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
        ?? Principal?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles =>
        Principal?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
        ?? new List<string>();
}