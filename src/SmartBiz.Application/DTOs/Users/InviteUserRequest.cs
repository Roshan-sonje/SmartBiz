namespace SmartBiz.Application.DTOs.Users;

public class InviteUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string TemporaryPassword { get; set; } = string.Empty;
    public List<Guid> RoleIds { get; set; } = new();
}