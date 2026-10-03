namespace SmartBiz.Application.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the given user.
    /// </summary>
    string GenerateAccessToken(
        Guid userId,
        Guid businessId,
        string email,
        string fullName,
        IEnumerable<string> roles);

    /// <summary>
    /// Generates a cryptographically random refresh token string.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Returns the SHA-256 hash of a refresh token (for storage).
    /// </summary>
    string HashRefreshToken(string token);

    /// <summary>
    /// Reads the expiry duration for access tokens (in minutes).
    /// </summary>
    int AccessTokenExpiryMinutes { get; }

    /// <summary>
    /// Reads the expiry duration for refresh tokens (in days).
    /// </summary>
    int RefreshTokenExpiryDays { get; }
}