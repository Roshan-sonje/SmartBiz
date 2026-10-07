namespace SmartBiz.Application.DTOs.Customers;

public class CustomerListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; }
}