using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.HasOne(b => b.Business)
            .WithMany()
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.BusinessId);
        builder.HasIndex(b => new { b.BusinessId, b.Name });
    }
}