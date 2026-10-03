using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(200);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.City).HasMaxLength(100);
        builder.Property(s => s.State).HasMaxLength(100);
        builder.Property(s => s.PostalCode).HasMaxLength(20);
        builder.Property(s => s.Gstin).HasMaxLength(20);
        builder.Property(s => s.Notes).HasMaxLength(1000);

        builder.Property(s => s.OpeningBalance)
            .HasPrecision(18, 2);

        builder.HasOne(s => s.Business)
            .WithMany()
            .HasForeignKey(s => s.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.BusinessId);
        builder.HasIndex(s => new { s.BusinessId, s.Phone });
        builder.HasIndex(s => new { s.BusinessId, s.Name });
    }
}