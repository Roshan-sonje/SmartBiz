using SmartBiz.Application.DTOs.Business;

namespace SmartBiz.Application.Interfaces;

public interface IBusinessService
{
    Task<BusinessProfileDto> GetProfileAsync(Guid businessId, CancellationToken ct = default);
    Task<BusinessProfileDto> UpdateProfileAsync(Guid businessId, UpdateBusinessProfileRequest request, CancellationToken ct = default);
    Task<BusinessSettingsDto> GetSettingsAsync(Guid businessId, CancellationToken ct = default);
    Task<BusinessSettingsDto> UpdateSettingsAsync(Guid businessId, UpdateBusinessSettingsRequest request, CancellationToken ct = default);
}