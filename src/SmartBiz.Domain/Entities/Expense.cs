using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class Expense : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public Guid ExpenseCategoryId { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Reference { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public ExpenseCategory ExpenseCategory { get; set; } = null!;
    public User? CreatedByUser { get; set; }
}