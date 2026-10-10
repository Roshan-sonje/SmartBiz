namespace SmartBiz.Desktop.Models.Units;

public class Unit
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool AllowDecimal { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}