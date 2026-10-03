using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("PurchaseItems");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ProductName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(pi => pi.ProductSku).HasMaxLength(50);

        builder.Property(pi => pi.Quantity).HasPrecision(18, 3);
        builder.Property(pi => pi.UnitCost).HasPrecision(18, 2);
        builder.Property(pi => pi.DiscountAmount).HasPrecision(18, 2);
        builder.Property(pi => pi.TaxRate).HasPrecision(5, 2);
        builder.Property(pi => pi.TaxAmount).HasPrecision(18, 2);
        builder.Property(pi => pi.LineTotal).HasPrecision(18, 2);

        builder.HasOne(pi => pi.Purchase)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Product)
            .WithMany()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pi => pi.PurchaseId);
        builder.HasIndex(pi => pi.ProductId);
    }
}