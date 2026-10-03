namespace SmartBiz.Application.Exceptions;

public class AuthException : Exception
{
    public int StatusCode { get; }

    public AuthException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public static AuthException InvalidCredentials() =>
        new("Invalid email or password.", 401);

    public static AuthException EmailAlreadyExists() =>
        new("An account with this email already exists.", 409);

    public static AuthException InvalidRefreshToken() =>
        new("Invalid or expired refresh token.", 401);

    public static AuthException UserNotActive() =>
        new("This account is not active.", 403);
}