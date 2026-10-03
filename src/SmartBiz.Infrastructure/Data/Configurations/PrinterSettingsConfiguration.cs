using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class PrinterSettingsConfiguration : IEntityTypeConfiguration<PrinterSettings>
{
    public void Configure(EntityTypeBuilder<PrinterSettings> builder)
    {
        builder.ToTable("PrinterSettings");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.PrinterName).HasMaxLength(200);
        builder.Property(ps => ps.PaperSize).HasConversion<int>();
        builder.Property(ps => ps.HeaderText).HasMaxLength(500);
        builder.Property(ps => ps.FooterText).HasMaxLength(500);

        // 1:1 with Business
        builder.HasOne(ps => ps.Business)
            .WithOne()
            .HasForeignKey<PrinterSettings>(ps => ps.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ps => ps.BusinessId).IsUnique();
    }
}