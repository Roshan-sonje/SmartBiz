using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .HasConversion<int>();

        // Unique: one email per business
        builder.HasIndex(u => new { u.BusinessId, u.Email }).IsUnique();

        // Foreign key to Business
        builder.HasOne(u => u.Business)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index on BusinessId for tenant filtering
        builder.HasIndex(u => u.BusinessId);
    }
}