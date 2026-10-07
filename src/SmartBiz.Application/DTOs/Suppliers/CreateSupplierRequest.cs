namespace SmartBiz.Application.DTOs.Suppliers;

public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Gstin { get; set; }
    public decimal OpeningBalance { get; set; }
    public string? Notes { get; set; }
}