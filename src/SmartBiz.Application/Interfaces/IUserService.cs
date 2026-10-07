using SmartBiz.Application.DTOs.Users;

namespace SmartBiz.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync(Guid businessId, CancellationToken ct = default);
    Task<UserDto> GetUserAsync(Guid businessId, Guid userId, CancellationToken ct = default);
    Task<UserDto> InviteUserAsync(Guid businessId, Guid invitedByUserId, InviteUserRequest request, CancellationToken ct = default);
    Task<UserDto> UpdateUserRolesAsync(Guid businessId, Guid userId, UpdateUserRolesRequest request, CancellationToken ct = default);
    Task DeactivateUserAsync(Guid businessId, Guid currentUserId, Guid userId, CancellationToken ct = default);
    Task<List<RoleDto>> GetRolesAsync(Guid businessId, CancellationToken ct = default);
}