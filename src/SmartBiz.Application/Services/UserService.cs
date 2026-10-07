using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.DTOs.Users;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetUsersAsync(Guid businessId, CancellationToken ct = default)
    {
        var users = await _db.Users
            .IgnoreQueryFilters()
            .Where(u => u.BusinessId == businessId)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.FullName)
            .AsNoTracking()
            .ToListAsync(ct);

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> GetUserAsync(Guid businessId, Guid userId, CancellationToken ct = default)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.BusinessId == businessId, ct);

        if (user is null)
            throw new AuthException("User not found.", 404);

        return MapToDto(user);
    }

    public async Task<UserDto> InviteUserAsync(
        Guid businessId,
        Guid invitedByUserId,
        InviteUserRequest request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Global uniqueness check
        var emailExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == email, ct);

        if (emailExists)
            throw AuthException.EmailAlreadyExists();

        // Validate all requested roles belong to this business
        var validRoleIds = await _db.Roles
            .IgnoreQueryFilters()
            .Where(r => r.BusinessId == businessId && request.RoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        if (validRoleIds.Count != request.RoleIds.Count)
            throw new AuthException("One or more roles are invalid for this business.", 400);

        var newUser = new User
        {
            BusinessId = businessId,
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            PasswordHash = _passwordHasher.Hash(request.TemporaryPassword),
            Status = UserStatus.Active
        };

        _db.Users.Add(newUser);

        foreach (var roleId in validRoleIds)
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = newUser.Id,
                RoleId = roleId
            });
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "User {Email} invited to business {BusinessId} by {InvitedByUserId}",
            email, businessId, invitedByUserId);

        // Re-query with roles included
        return await GetUserAsync(businessId, newUser.Id, ct);
    }

    public async Task<UserDto> UpdateUserRolesAsync(
        Guid businessId,
        Guid userId,
        UpdateUserRolesRequest request,
        CancellationToken ct = default)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId && u.BusinessId == businessId, ct);

        if (user is null)
            throw new AuthException("User not found.", 404);

        // Validate roles
        var validRoleIds = await _db.Roles
            .IgnoreQueryFilters()
            .Where(r => r.BusinessId == businessId && request.RoleIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        if (validRoleIds.Count != request.RoleIds.Count)
            throw new AuthException("One or more roles are invalid for this business.", 400);

        // Remove existing roles
        _db.UserRoles.RemoveRange(user.UserRoles);

        // Add new roles
        foreach (var roleId in validRoleIds)
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Roles updated for user {UserId}", userId);

        return await GetUserAsync(businessId, userId, ct);
    }

    public async Task DeactivateUserAsync(
        Guid businessId,
        Guid currentUserId,
        Guid userId,
        CancellationToken ct = default)
    {
        if (currentUserId == userId)
            throw new AuthException("You cannot deactivate your own account.", 400);

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && u.BusinessId == businessId, ct);

        if (user is null)
            throw new AuthException("User not found.", 404);

        user.Status = UserStatus.Inactive;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} deactivated", userId);
    }

    public async Task<List<RoleDto>> GetRolesAsync(Guid businessId, CancellationToken ct = default)
    {
        var roles = await _db.Roles
            .IgnoreQueryFilters()
            .Where(r => r.BusinessId == businessId)
            .OrderBy(r => r.Name)
            .AsNoTracking()
            .ToListAsync(ct);

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsSystemRole = r.IsSystemRole
        }).ToList();
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private static UserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        Status = u.Status.ToString(),
        LastLoginAt = u.LastLoginAt,
        CreatedAt = u.CreatedAt,
        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
    };
}