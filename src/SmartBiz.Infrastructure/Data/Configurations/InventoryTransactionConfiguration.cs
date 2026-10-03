using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");

        builder.HasKey(it => it.Id);

        builder.Property(it => it.Type).HasConversion<int>();
        builder.Property(it => it.Quantity).HasPrecision(18, 3);
        builder.Property(it => it.BalanceAfter).HasPrecision(18, 3);
        builder.Property(it => it.ReferenceType).HasMaxLength(50);
        builder.Property(it => it.Notes).HasMaxLength(1000);

        builder.HasOne(it => it.Business)
            .WithMany()
            .HasForeignKey(it => it.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(it => it.Product)
            .WithMany()
            .HasForeignKey(it => it.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(it => it.CreatedByUser)
            .WithMany()
            .HasForeignKey(it => it.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(it => it.BusinessId);
        builder.HasIndex(it => new { it.BusinessId, it.ProductId, it.CreatedAt });
        builder.HasIndex(it => it.ReferenceId);
    }
}