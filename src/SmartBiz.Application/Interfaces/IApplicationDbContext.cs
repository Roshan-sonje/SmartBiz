using Microsoft.EntityFrameworkCore;
using SmartBiz.Domain.Entities;

namespace SmartBiz.Application.Interfaces;

/// <summary>
/// Abstraction over the EF Core DbContext so the Application layer
/// doesn't reference SmartBiz.Infrastructure directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Business> Businesses { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Customer> Customers { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Unit> Units { get; }
    DbSet<Tax> Taxes { get; }
    DbSet<Product> Products { get; }

    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Purchase> Purchases { get; }
    DbSet<PurchaseItem> PurchaseItems { get; }

    DbSet<Payment> Payments { get; }
    DbSet<ExpenseCategory> ExpenseCategories { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<StockAdjustment> StockAdjustments { get; }

    DbSet<BusinessSettings> BusinessSettings { get; }
    DbSet<PrinterSettings> PrinterSettings { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}