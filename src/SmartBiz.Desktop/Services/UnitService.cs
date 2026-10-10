using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Common;
using SmartBiz.Desktop.Models.Units;

namespace SmartBiz.Desktop.Services;

public class UnitService : IUnitService
{
    private readonly IApiClient _api;
    private readonly ILogger<UnitService> _logger;

    public UnitService(IApiClient api, ILogger<UnitService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<PagedResult<UnitListItem>> GetUnitsAsync(
        int page = 1, int pageSize = 100, string? search = null,
        bool includeInactive = false, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}&includeInactive={includeInactive.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        var result = await _api.GetAsync<PagedResult<UnitListItem>>(
            $"/api/{AppConfig.ApiVersion}/units{query}", ct);

        return result ?? new PagedResult<UnitListItem>();
    }

    public async Task<Unit> CreateUnitAsync(CreateUnitRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<CreateUnitRequest, Unit>(
            $"/api/{AppConfig.ApiVersion}/units", request, ct);
        return result ?? throw new InvalidOperationException("Failed to create unit.");
    }

    public async Task DeleteUnitAsync(Guid id, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/units/{id}", ct);
        _logger.LogInformation("Unit deleted: {Id}", id);
    }
}