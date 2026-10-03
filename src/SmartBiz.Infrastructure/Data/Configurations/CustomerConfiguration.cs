using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.Property(c => c.PostalCode).HasMaxLength(20);
        builder.Property(c => c.Gstin).HasMaxLength(20);
        builder.Property(c => c.Notes).HasMaxLength(1000);

        builder.Property(c => c.OpeningBalance)
            .HasPrecision(18, 2);

        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 2);

        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.BusinessId);
        builder.HasIndex(c => new { c.BusinessId, c.Phone });
        builder.HasIndex(c => new { c.BusinessId, c.Name });
    }
}