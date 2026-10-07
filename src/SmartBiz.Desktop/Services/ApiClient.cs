using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Auth;

namespace SmartBiz.Desktop.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    public ApiClient(HttpClient http, ITokenStore tokenStore, ILogger<ApiClient> logger)
    {
        _http = http;
        _tokenStore = tokenStore;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Get, endpoint, null, ct);

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default)
        => SendAsync<TResponse>(HttpMethod.Post, endpoint, data, ct);

    public async Task PostAsync<TRequest>(string endpoint, TRequest data, CancellationToken ct = default)
        => await SendAsync<object>(HttpMethod.Post, endpoint, data, ct);

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default)
        => SendAsync<TResponse>(HttpMethod.Put, endpoint, data, ct);

    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
        => await SendAsync<object>(HttpMethod.Delete, endpoint, null, ct);

    private async Task<T?> SendAsync<T>(HttpMethod method, string endpoint, object? body, CancellationToken ct)
    {
        await EnsureFreshTokenAsync(ct);

        var request = new HttpRequestMessage(method, endpoint);
        if (body is not null) request.Content = JsonContent.Create(body, options: _jsonOptions);

        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (await TryRefreshAsync(ct))
                return await SendAsync<T>(method, endpoint, body, ct);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("API error {Status} on {Method} {Url}: {Body}",
                (int)response.StatusCode, method, endpoint, errorBody);

            var msg = TryExtractErrorMessage(errorBody) ?? $"Request failed with status {(int)response.StatusCode}.";
            throw new ApiException(response.StatusCode, msg, errorBody);
        }

        if (typeof(T) == typeof(object) || response.StatusCode == HttpStatusCode.NoContent)
            return default;

        var content = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(content)) return default;

        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    private async Task EnsureFreshTokenAsync(CancellationToken ct)
    {
        var expiry = await _tokenStore.GetAccessTokenExpiryAsync();
        if (expiry is null) return;
        if (expiry.Value <= DateTime.UtcNow.AddSeconds(60))
            await TryRefreshAsync(ct);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken ct)
    {
        await _refreshLock.WaitAsync(ct);
        try
        {
            var refreshToken = await _tokenStore.GetRefreshTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshToken)) return false;

            var req = new RefreshRequest { RefreshToken = refreshToken };
            var response = await _http.PostAsJsonAsync($"/api/{AppConfig.ApiVersion}/auth/refresh", req, _jsonOptions, ct);

            if (!response.IsSuccessStatusCode)
            {
                await _tokenStore.ClearAsync();
                return false;
            }

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions, ct);
            if (auth is null) return false;

            await _tokenStore.SaveTokensAsync(auth);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static string? TryExtractErrorMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                return detail.GetString();
            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                return title.GetString();
            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in errors.EnumerateObject())
                    if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                        return prop.Value[0].GetString();
            }
        }
        catch { }
        return null;
    }
}