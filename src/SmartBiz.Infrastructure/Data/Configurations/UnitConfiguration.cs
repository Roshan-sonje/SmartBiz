using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ShortName)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(u => u.Business)
            .WithMany()
            .HasForeignKey(u => u.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.BusinessId);
        builder.HasIndex(u => new { u.BusinessId, u.ShortName }).IsUnique();
    }
}