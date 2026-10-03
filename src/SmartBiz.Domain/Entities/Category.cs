using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Category : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Self-referencing: categories can have a parent
    public Guid? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = null!;
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}