using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class BusinessSettingsConfiguration : IEntityTypeConfiguration<BusinessSettings>
{
    public void Configure(EntityTypeBuilder<BusinessSettings> builder)
    {
        builder.ToTable("BusinessSettings");

        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.InvoicePrefix)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(bs => bs.InvoiceNumberFormat)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bs => bs.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(bs => bs.CurrencySymbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(bs => bs.DecimalSeparator)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(bs => bs.TermsAndConditions).HasMaxLength(2000);
        builder.Property(bs => bs.InvoiceFooterNote).HasMaxLength(500);

        // 1:1 with Business — enforced unique
        builder.HasOne(bs => bs.Business)
            .WithOne()
            .HasForeignKey<BusinessSettings>(bs => bs.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bs => bs.DefaultTax)
            .WithMany()
            .HasForeignKey(bs => bs.DefaultTaxId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(bs => bs.BusinessId).IsUnique();
    }
}