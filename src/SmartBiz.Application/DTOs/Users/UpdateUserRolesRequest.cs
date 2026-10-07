namespace SmartBiz.Application.DTOs.Users;

public class UpdateUserRolesRequest
{
    public List<Guid> RoleIds { get; set; } = new();
}