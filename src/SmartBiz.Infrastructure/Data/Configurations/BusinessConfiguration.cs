using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.LegalName)
            .HasMaxLength(200);

        builder.Property(b => b.Gstin)
            .HasMaxLength(20);

        builder.Property(b => b.Phone)
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasMaxLength(200);

        builder.Property(b => b.Address)
            .HasMaxLength(500);

        builder.Property(b => b.City)
            .HasMaxLength(100);

        builder.Property(b => b.State)
            .HasMaxLength(100);

        builder.Property(b => b.PostalCode)
            .HasMaxLength(20);

        builder.Property(b => b.Country)
            .HasMaxLength(100);

        builder.Property(b => b.LogoUrl)
            .HasMaxLength(500);

        builder.Property(b => b.Currency)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("INR");

        builder.Property(b => b.Status)
            .HasConversion<int>();

        builder.HasIndex(b => b.Name);
        builder.HasIndex(b => b.Status);
    }
}