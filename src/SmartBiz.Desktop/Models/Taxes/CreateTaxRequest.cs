namespace SmartBiz.Desktop.Models.Taxes;

public class CreateTaxRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string? Description { get; set; }
}