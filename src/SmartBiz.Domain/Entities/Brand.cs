using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Brand : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}