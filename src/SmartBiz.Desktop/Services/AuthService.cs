using System.Net;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Auth;

namespace SmartBiz.Desktop.Services;

public class AuthService : IAuthService
{
    private readonly IApiClient _api;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IApiClient api, ITokenStore tokenStore, ILogger<AuthService> logger)
    {
        _api = api;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var request = new LoginRequest { Email = email, Password = password };
        var response = await _api.PostAsync<LoginRequest, AuthResponse>(
            $"/api/{AppConfig.ApiVersion}/auth/login", request, ct);

        if (response is null)
            throw new ApiException(HttpStatusCode.InternalServerError, "Login returned empty response.");

        await _tokenStore.SaveTokensAsync(response);
        _logger.LogInformation("User logged in: {Email}", response.Email);
        return response;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _api.PostAsync<RegisterRequest, AuthResponse>(
            $"/api/{AppConfig.ApiVersion}/auth/register", request, ct);

        if (response is null)
            throw new ApiException(HttpStatusCode.InternalServerError, "Register returned empty response.");

        await _tokenStore.SaveTokensAsync(response);
        _logger.LogInformation("New user registered: {Email}", response.Email);
        return response;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            var refreshToken = await _tokenStore.GetRefreshTokenAsync();
            if (!string.IsNullOrWhiteSpace(refreshToken))
                await _api.PostAsync<RefreshRequest>(
                    $"/api/{AppConfig.ApiVersion}/auth/logout",
                    new RefreshRequest { RefreshToken = refreshToken }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout API call failed — clearing local tokens anyway");
        }
        finally
        {
            await _tokenStore.ClearAsync();
        }
    }

    public Task<bool> IsAuthenticatedAsync() => _tokenStore.HasValidSessionAsync();
    public Task<string?> GetCurrentUserEmailAsync() => _tokenStore.GetEmailAsync();
    public Task<string?> GetCurrentUserFullNameAsync() => _tokenStore.GetFullNameAsync();
}