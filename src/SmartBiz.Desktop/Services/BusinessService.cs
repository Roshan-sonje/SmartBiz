using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Business;

namespace SmartBiz.Desktop.Services;

public class BusinessService : IBusinessService
{
    private readonly IApiClient _api;
    private readonly ILogger<BusinessService> _logger;

    public BusinessService(IApiClient api, ILogger<BusinessService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<BusinessProfile> GetProfileAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<BusinessProfile>(
            $"/api/{AppConfig.ApiVersion}/business/profile", ct);

        return result ?? throw new InvalidOperationException("Failed to load business profile.");
    }

    public async Task<BusinessProfile> UpdateProfileAsync(
        UpdateBusinessProfileRequest request,
        CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateBusinessProfileRequest, BusinessProfile>(
            $"/api/{AppConfig.ApiVersion}/business/profile", request, ct);

        _logger.LogInformation("Business profile updated");
        return result ?? throw new InvalidOperationException("Failed to update business profile.");
    }

    public async Task<BusinessSettings> GetSettingsAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<BusinessSettings>(
            $"/api/{AppConfig.ApiVersion}/business/settings", ct);

        return result ?? throw new InvalidOperationException("Failed to load settings.");
    }

    public async Task<BusinessSettings> UpdateSettingsAsync(
        UpdateBusinessSettingsRequest request,
        CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateBusinessSettingsRequest, BusinessSettings>(
            $"/api/{AppConfig.ApiVersion}/business/settings", request, ct);

        _logger.LogInformation("Business settings updated");
        return result ?? throw new InvalidOperationException("Failed to update settings.");
    }
}