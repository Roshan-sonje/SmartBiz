namespace SmartBiz.Desktop.Models.Units;

public class CreateUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool AllowDecimal { get; set; }
}