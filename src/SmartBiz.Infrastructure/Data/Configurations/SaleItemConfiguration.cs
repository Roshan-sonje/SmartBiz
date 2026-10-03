using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.ProductName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(si => si.ProductSku).HasMaxLength(50);

        builder.Property(si => si.Quantity).HasPrecision(18, 3);
        builder.Property(si => si.UnitPrice).HasPrecision(18, 2);
        builder.Property(si => si.DiscountAmount).HasPrecision(18, 2);
        builder.Property(si => si.TaxRate).HasPrecision(5, 2);
        builder.Property(si => si.TaxAmount).HasPrecision(18, 2);
        builder.Property(si => si.LineTotal).HasPrecision(18, 2);

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(si => si.SaleId);
        builder.HasIndex(si => si.ProductId);
    }
}