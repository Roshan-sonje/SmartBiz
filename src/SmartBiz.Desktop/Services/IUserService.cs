using SmartBiz.Desktop.Models.Users;

namespace SmartBiz.Desktop.Services;

public interface IUserService
{
    Task<List<User>> GetUsersAsync(CancellationToken ct = default);
    Task<User> InviteUserAsync(InviteUserRequest request, CancellationToken ct = default);
    Task<User> UpdateUserRolesAsync(Guid userId, UpdateUserRolesRequest request, CancellationToken ct = default);
    Task DeactivateUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<Role>> GetRolesAsync(CancellationToken ct = default);
}