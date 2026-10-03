using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Notes).HasMaxLength(2000);

        builder.Property(s => s.Subtotal).HasPrecision(18, 2);
        builder.Property(s => s.DiscountAmount).HasPrecision(18, 2);
        builder.Property(s => s.TaxAmount).HasPrecision(18, 2);
        builder.Property(s => s.RoundOff).HasPrecision(18, 2);
        builder.Property(s => s.GrandTotal).HasPrecision(18, 2);
        builder.Property(s => s.PaidAmount).HasPrecision(18, 2);
        builder.Property(s => s.DueAmount).HasPrecision(18, 2);

        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.PaymentStatus).HasConversion<int>();

        builder.HasOne(s => s.Business)
            .WithMany()
            .HasForeignKey(s => s.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.CreatedByUser)
            .WithMany()
            .HasForeignKey(s => s.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.BusinessId);
        builder.HasIndex(s => new { s.BusinessId, s.InvoiceNumber }).IsUnique();
        builder.HasIndex(s => new { s.BusinessId, s.InvoiceDate });
        builder.HasIndex(s => new { s.BusinessId, s.CustomerId });
    }
}