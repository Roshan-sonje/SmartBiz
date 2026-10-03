using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Unit : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public string Name { get; set; } = string.Empty;      // e.g. "Kilogram"
    public string ShortName { get; set; } = string.Empty; // e.g. "kg"

    public bool AllowDecimal { get; set; } = false;       // can you sell 1.5 kg? Yes / No

    public bool IsActive { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}