using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("Taxes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Rate)
            .HasPrecision(5, 2);

        builder.HasOne(t => t.Business)
            .WithMany()
            .HasForeignKey(t => t.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.BusinessId);
    }
}