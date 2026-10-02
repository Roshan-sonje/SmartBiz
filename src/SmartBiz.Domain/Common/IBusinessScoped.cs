namespace SmartBiz.Domain.Common;

public interface IBusinessScoped
{
    Guid BusinessId { get; set; }
}