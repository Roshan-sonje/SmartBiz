using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class Invoice : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    public Guid SaleId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public string? PdfUrl { get; set; }
    public string? TemplateCode { get; set; }

    public int PrintCount { get; set; }
    public DateTime? LastPrintedAt { get; set; }

    // Navigation
    public Business Business { get; set; } = null!;
    public Sale Sale { get; set; } = null!;
}