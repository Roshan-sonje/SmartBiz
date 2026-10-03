using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.EntityType).HasMaxLength(100);
        builder.Property(al => al.IpAddress).HasMaxLength(50);
        builder.Property(al => al.UserAgent).HasMaxLength(500);

        // JSONB in PostgreSQL for structured old/new value storage
        builder.Property(al => al.OldValues).HasColumnType("jsonb");
        builder.Property(al => al.NewValues).HasColumnType("jsonb");
        builder.Property(al => al.AdditionalData).HasColumnType("jsonb");

        builder.HasOne(al => al.Business)
            .WithMany()
            .HasForeignKey(al => al.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(al => al.BusinessId);
        builder.HasIndex(al => new { al.BusinessId, al.CreatedAt });
        builder.HasIndex(al => new { al.BusinessId, al.Action });
        builder.HasIndex(al => new { al.EntityType, al.EntityId });
    }
}