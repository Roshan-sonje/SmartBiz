using SmartBiz.Domain.Common;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Domain.Entities;

public class PrinterSettings : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public string? PrinterName { get; set; }
    public PaperSize PaperSize { get; set; } = PaperSize.A4;
    public int Copies { get; set; } = 1;

    public bool ShowLogo { get; set; } = true;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }

    public bool AutoPrintAfterSale { get; set; } = false;

    // Navigation
    public Business Business { get; set; } = null!;
}