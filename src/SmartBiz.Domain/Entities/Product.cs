using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Product : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid UnitId { get; set; }
    public Guid? TaxId { get; set; }
    public Guid? PreferredSupplierId { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }

    public decimal StockQuantity { get; set; }
    public decimal MinStockLevel { get; set; }

    public bool IsActive { get; set; } = true;
    public bool TrackInventory { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = null!;
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public Unit Unit { get; set; } = null!;
    public Tax? Tax { get; set; }
    public Supplier? PreferredSupplier { get; set; }
}