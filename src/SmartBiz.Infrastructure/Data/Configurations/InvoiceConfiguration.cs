using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.PdfUrl).HasMaxLength(500);
        builder.Property(i => i.TemplateCode).HasMaxLength(50);

        builder.HasOne(i => i.Business)
            .WithMany()
            .HasForeignKey(i => i.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        // One invoice per sale
        builder.HasOne(i => i.Sale)
            .WithOne(s => s.Invoice)
            .HasForeignKey<Invoice>(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.BusinessId);
        builder.HasIndex(i => i.SaleId).IsUnique();
        builder.HasIndex(i => new { i.BusinessId, i.InvoiceNumber }).IsUnique();
    }
}