namespace SmartBiz.Desktop.Models.Users;

public class UpdateUserRolesRequest
{
    public List<Guid> RoleIds { get; set; } = new();
}