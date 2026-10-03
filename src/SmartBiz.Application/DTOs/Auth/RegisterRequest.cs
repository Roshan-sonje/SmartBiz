namespace SmartBiz.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessGstin { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessAddress { get; set; }
}