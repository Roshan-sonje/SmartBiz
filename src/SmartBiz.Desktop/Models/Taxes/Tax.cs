namespace SmartBiz.Desktop.Models.Taxes;

public class Tax
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}