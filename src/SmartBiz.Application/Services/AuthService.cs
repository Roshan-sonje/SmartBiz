using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.DTOs.Auth;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(
    RegisterRequest request,
    string? ipAddress,
    CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Global email check — one email = one account
        var emailExists = await _db.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == email, ct);

        if (emailExists)
        {
            _logger.LogWarning("Registration attempted with existing email: {Email}", email);
            throw AuthException.EmailAlreadyExists();
        }
        // 1. Business name must be unique enough to create a tenant
        // (Simplified: allow duplicate business names for now — can add uniqueness later)

        // 2. Create Business
        var business = new Business
        {
            Name = request.BusinessName.Trim(),
            Gstin = string.IsNullOrWhiteSpace(request.BusinessGstin) ? null : request.BusinessGstin.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.BusinessPhone) ? null : request.BusinessPhone.Trim(),
            Address = string.IsNullOrWhiteSpace(request.BusinessAddress) ? null : request.BusinessAddress.Trim(),
            Email = email,
            Status = BusinessStatus.Trial,
            TrialEndsAt = DateTime.UtcNow.AddDays(30),
            Currency = "INR"
        };

        _db.Businesses.Add(business);

        // 3. Create User
        var user = new User
        {
            BusinessId = business.Id,
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            Status = UserStatus.Active
        };

        _db.Users.Add(user);

        // 4. Create default "Admin" role for this business
        var adminRole = new Role
        {
            BusinessId = business.Id,
            Name = "Admin",
            Description = "Full access to all features.",
            IsSystemRole = true
        };

        _db.Roles.Add(adminRole);

        // 5. Assign user to Admin role
        _db.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id
        });

        // 6. Create default BusinessSettings
        _db.BusinessSettings.Add(new BusinessSettings
        {
            BusinessId = business.Id,
            InvoicePrefix = "INV",
            InvoiceNextNumber = 1,
            IsGstEnabled = true,
            Currency = "INR",
            CurrencySymbol = "₹"
        });

        // 7. Persist everything in one transaction
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "New business registered: {BusinessId} ({BusinessName}), user {Email}",
            business.Id, business.Name, email);

        // 8. Generate tokens and return
        return await BuildAuthResponseAsync(
            user,
            business,
            new[] { "Admin" },
            ipAddress,
            ct);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .IgnoreQueryFilters()   // login: we don't yet know the tenant
            .Include(u => u.Business)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
        {
            _logger.LogWarning("Login failed for unknown email: {Email}", email);
            throw AuthException.InvalidCredentials();
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed for email: {Email} (bad password)", email);
            throw AuthException.InvalidCredentials();
        }

        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Login attempt for inactive user: {Email}", email);
            throw AuthException.UserNotActive();
        }

        user.LastLoginAt = DateTime.UtcNow;

        var roles = user.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToArray();

        return await BuildAuthResponseAsync(user, user.Business, roles, ipAddress, ct);
    }

    public async Task<AuthResponse> RefreshAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw AuthException.InvalidRefreshToken();

        var tokenHash = _tokenService.HashRefreshToken(refreshToken);

        var existing = await _db.RefreshTokens
            .IgnoreQueryFilters()
            .Include(rt => rt.User)
                .ThenInclude(u => u.Business)
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is null || !existing.IsActive)
        {
            _logger.LogWarning("Refresh token invalid or expired");
            throw AuthException.InvalidRefreshToken();
        }

        // Rotate: revoke old token, issue new one
        existing.RevokedAt = DateTime.UtcNow;
        existing.RevokedByIp = ipAddress;

        var roles = existing.User.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToArray();

        return await BuildAuthResponseAsync(
            existing.User,
            existing.User.Business,
            roles,
            ipAddress,
            ct);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;   // idempotent

        var tokenHash = _tokenService.HashRefreshToken(refreshToken);

        var existing = await _db.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existing is null || existing.RevokedAt is not null)
            return;   // already revoked or unknown — logout is idempotent

        existing.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Refresh token revoked for user {UserId}", existing.UserId);
    }

    // -------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------

    private async Task<AuthResponse> BuildAuthResponseAsync(
        User user,
        Business business,
        IEnumerable<string> roles,
        string? ipAddress,
        CancellationToken ct)
    {
        var roleList = roles.ToList();

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            business.Id,
            user.Email,
            user.FullName,
            roleList);

        var refreshTokenRaw = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshTokenRaw);

        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenService.AccessTokenExpiryMinutes);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpiryDays);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = refreshExpiresAt,
            CreatedByIp = ipAddress
        });

        await _db.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenRaw,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshTokenExpiresAt = refreshExpiresAt,
            UserId = user.Id,
            BusinessId = business.Id,
            Email = user.Email,
            FullName = user.FullName,
            BusinessName = business.Name,
            Roles = roleList
        };
    }
}