using System.Text.Json;
using SmartBiz.Desktop.Models.Auth;

namespace SmartBiz.Desktop.Services;

public class TokenStore : ITokenStore
{
    private const string AccessTokenKey = "smartbiz_access_token";
    private const string RefreshTokenKey = "smartbiz_refresh_token";
    private const string AccessExpiryKey = "smartbiz_access_expiry";
    private const string BusinessIdKey = "smartbiz_business_id";
    private const string UserIdKey = "smartbiz_user_id";
    private const string EmailKey = "smartbiz_email";
    private const string FullNameKey = "smartbiz_full_name";
    private const string RolesKey = "smartbiz_roles";

    public async Task SaveTokensAsync(AuthResponse auth)
    {
        await SecureStorage.SetAsync(AccessTokenKey, auth.AccessToken);
        await SecureStorage.SetAsync(RefreshTokenKey, auth.RefreshToken);
        await SecureStorage.SetAsync(AccessExpiryKey, auth.AccessTokenExpiresAt.ToString("O"));
        await SecureStorage.SetAsync(BusinessIdKey, auth.BusinessId.ToString());
        await SecureStorage.SetAsync(UserIdKey, auth.UserId.ToString());
        await SecureStorage.SetAsync(EmailKey, auth.Email);
        await SecureStorage.SetAsync(FullNameKey, auth.FullName);
        await SecureStorage.SetAsync(RolesKey, JsonSerializer.Serialize(auth.Roles));
    }

    public async Task<string?> GetAccessTokenAsync() => await SafeGetAsync(AccessTokenKey);
    public async Task<string?> GetRefreshTokenAsync() => await SafeGetAsync(RefreshTokenKey);

    public async Task<DateTime?> GetAccessTokenExpiryAsync()
    {
        var raw = await SafeGetAsync(AccessExpiryKey);
        return DateTime.TryParse(raw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt)
            ? dt : null;
    }

    public async Task<Guid?> GetBusinessIdAsync()
    {
        var raw = await SafeGetAsync(BusinessIdKey);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    public async Task<Guid?> GetUserIdAsync()
    {
        var raw = await SafeGetAsync(UserIdKey);
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    public async Task<string?> GetEmailAsync() => await SafeGetAsync(EmailKey);
    public async Task<string?> GetFullNameAsync() => await SafeGetAsync(FullNameKey);

    public async Task<List<string>> GetRolesAsync()
    {
        var raw = await SafeGetAsync(RolesKey);
        if (string.IsNullOrWhiteSpace(raw)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(raw) ?? new List<string>(); }
        catch { return new List<string>(); }
    }

    public Task ClearAsync()
    {
        SecureStorage.Remove(AccessTokenKey);
        SecureStorage.Remove(RefreshTokenKey);
        SecureStorage.Remove(AccessExpiryKey);
        SecureStorage.Remove(BusinessIdKey);
        SecureStorage.Remove(UserIdKey);
        SecureStorage.Remove(EmailKey);
        SecureStorage.Remove(FullNameKey);
        SecureStorage.Remove(RolesKey);
        return Task.CompletedTask;
    }

    public async Task<bool> HasValidSessionAsync()
    {
        var token = await GetAccessTokenAsync();
        var expiry = await GetAccessTokenExpiryAsync();
        if (string.IsNullOrWhiteSpace(token) || expiry is null) return false;
        return expiry.Value > DateTime.UtcNow.AddSeconds(30);
    }

    private static async Task<string?> SafeGetAsync(string key)
    {
        try { return await SecureStorage.GetAsync(key); }
        catch { return null; }
    }
}