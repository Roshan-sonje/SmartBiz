using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Models.Users;

namespace SmartBiz.Desktop.Services;

public class UserService : IUserService
{
    private readonly IApiClient _api;
    private readonly ILogger<UserService> _logger;

    public UserService(IApiClient api, ILogger<UserService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<List<User>> GetUsersAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<List<User>>(
            $"/api/{AppConfig.ApiVersion}/users", ct);
        return result ?? new List<User>();
    }

    public async Task<User> InviteUserAsync(InviteUserRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<InviteUserRequest, User>(
            $"/api/{AppConfig.ApiVersion}/users/invite", request, ct);

        _logger.LogInformation("User invited: {Email}", request.Email);
        return result ?? throw new InvalidOperationException("Failed to invite user.");
    }

    public async Task<User> UpdateUserRolesAsync(Guid userId, UpdateUserRolesRequest request, CancellationToken ct = default)
    {
        var result = await _api.PutAsync<UpdateUserRolesRequest, User>(
            $"/api/{AppConfig.ApiVersion}/users/{userId}/roles", request, ct);

        _logger.LogInformation("Roles updated for user {UserId}", userId);
        return result ?? throw new InvalidOperationException("Failed to update roles.");
    }

    public async Task DeactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        await _api.DeleteAsync($"/api/{AppConfig.ApiVersion}/users/{userId}", ct);
        _logger.LogInformation("User deactivated: {UserId}", userId);
    }

    public async Task<List<Role>> GetRolesAsync(CancellationToken ct = default)
    {
        var result = await _api.GetAsync<List<Role>>(
            $"/api/{AppConfig.ApiVersion}/roles", ct);
        return result ?? new List<Role>();
    }
}