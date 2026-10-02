namespace SmartBiz.Application.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? BusinessId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}