using SmartBiz.Desktop.Models.Auth;

namespace SmartBiz.Desktop.Services;

public interface ITokenStore
{
    Task SaveTokensAsync(AuthResponse auth);
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<DateTime?> GetAccessTokenExpiryAsync();
    Task<Guid?> GetBusinessIdAsync();
    Task<Guid?> GetUserIdAsync();
    Task<string?> GetEmailAsync();
    Task<string?> GetFullNameAsync();
    Task<List<string>> GetRolesAsync();
    Task ClearAsync();
    Task<bool> HasValidSessionAsync();
}