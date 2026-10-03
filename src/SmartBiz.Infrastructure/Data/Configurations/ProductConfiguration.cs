using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Barcode)
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.SellingPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.StockQuantity)
            .HasPrecision(18, 3);

        builder.Property(p => p.MinStockLevel)
            .HasPrecision(18, 3);

        // Business
        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category (optional)
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Brand (optional)
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        // Unit (required)
        builder.HasOne(p => p.Unit)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tax (optional)
        builder.HasOne(p => p.Tax)
            .WithMany(t => t.Products)
            .HasForeignKey(p => p.TaxId)
            .OnDelete(DeleteBehavior.SetNull);

        // Preferred supplier (optional)
        builder.HasOne(p => p.PreferredSupplier)
            .WithMany()
            .HasForeignKey(p => p.PreferredSupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.BusinessId);

        // Unique SKU per business
        builder.HasIndex(p => new { p.BusinessId, p.Sku }).IsUnique();

        // Barcode: unique per business when present (partial index handled by EF)
        builder.HasIndex(p => new { p.BusinessId, p.Barcode });

        builder.HasIndex(p => new { p.BusinessId, p.Name });
        builder.HasIndex(p => new { p.BusinessId, p.IsActive });
    }
}