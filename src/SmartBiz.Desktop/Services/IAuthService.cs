using SmartBiz.Desktop.Models.Auth;

namespace SmartBiz.Desktop.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetCurrentUserEmailAsync();
    Task<string?> GetCurrentUserFullNameAsync();
}