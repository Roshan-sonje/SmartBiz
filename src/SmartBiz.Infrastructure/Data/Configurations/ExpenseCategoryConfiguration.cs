using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Infrastructure.Data.Configurations;

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");

        builder.HasKey(ec => ec.Id);

        builder.Property(ec => ec.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(ec => ec.Description)
            .HasMaxLength(500);

        builder.HasOne(ec => ec.Business)
            .WithMany()
            .HasForeignKey(ec => ec.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ec => ec.BusinessId);
        builder.HasIndex(ec => new { ec.BusinessId, ec.Name });
    }
}