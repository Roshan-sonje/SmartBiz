using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Type).HasConversion<int>();
        builder.Property(p => p.Method).HasConversion<int>();

        builder.Property(p => p.Reference).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(2000);

        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Sale)
            .WithMany()
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Purchase)
            .WithMany()
            .HasForeignKey(p => p.PurchaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.BusinessId);
        builder.HasIndex(p => new { p.BusinessId, p.PaymentDate });
        builder.HasIndex(p => new { p.BusinessId, p.CustomerId });
        builder.HasIndex(p => new { p.BusinessId, p.SupplierId });
        builder.HasIndex(p => p.SaleId);
        builder.HasIndex(p => p.PurchaseId);
    }
}