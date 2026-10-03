using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ReferenceNumber).HasMaxLength(50);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        builder.Property(p => p.Subtotal).HasPrecision(18, 2);
        builder.Property(p => p.DiscountAmount).HasPrecision(18, 2);
        builder.Property(p => p.TaxAmount).HasPrecision(18, 2);
        builder.Property(p => p.RoundOff).HasPrecision(18, 2);
        builder.Property(p => p.GrandTotal).HasPrecision(18, 2);
        builder.Property(p => p.PaidAmount).HasPrecision(18, 2);
        builder.Property(p => p.DueAmount).HasPrecision(18, 2);

        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.PaymentStatus).HasConversion<int>();

        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.BusinessId);
        builder.HasIndex(p => new { p.BusinessId, p.PurchaseDate });
        builder.HasIndex(p => new { p.BusinessId, p.SupplierId });
    }
}