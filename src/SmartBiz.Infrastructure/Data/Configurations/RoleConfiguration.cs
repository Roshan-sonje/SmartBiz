using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        // Unique: one role name per business
        builder.HasIndex(r => new { r.BusinessId, r.Name }).IsUnique();

        builder.HasOne(r => r.Business)
            .WithMany(b => b.Roles)
            .HasForeignKey(r => r.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}