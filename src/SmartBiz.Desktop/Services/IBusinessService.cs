using SmartBiz.Desktop.Models.Business;

namespace SmartBiz.Desktop.Services;

public interface IBusinessService
{
    Task<BusinessProfile> GetProfileAsync(CancellationToken ct = default);
    Task<BusinessProfile> UpdateProfileAsync(UpdateBusinessProfileRequest request, CancellationToken ct = default);
    Task<BusinessSettings> GetSettingsAsync(CancellationToken ct = default);
    Task<BusinessSettings> UpdateSettingsAsync(UpdateBusinessSettingsRequest request, CancellationToken ct = default);
}