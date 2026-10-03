using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("StockAdjustments");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.PreviousQuantity).HasPrecision(18, 3);
        builder.Property(sa => sa.NewQuantity).HasPrecision(18, 3);
        builder.Property(sa => sa.Difference).HasPrecision(18, 3);

        builder.Property(sa => sa.Reason)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sa => sa.Notes).HasMaxLength(1000);

        builder.HasOne(sa => sa.Business)
            .WithMany()
            .HasForeignKey(sa => sa.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sa => sa.Product)
            .WithMany()
            .HasForeignKey(sa => sa.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sa => sa.CreatedByUser)
            .WithMany()
            .HasForeignKey(sa => sa.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(sa => sa.BusinessId);
        builder.HasIndex(sa => new { sa.BusinessId, sa.ProductId });
        builder.HasIndex(sa => new { sa.BusinessId, sa.CreatedAt });
    }
}