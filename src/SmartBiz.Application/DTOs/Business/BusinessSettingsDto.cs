namespace SmartBiz.Application.DTOs.Business;

public class BusinessSettingsDto
{
    public Guid Id { get; set; }
    public string InvoicePrefix { get; set; } = "INV";
    public long InvoiceNextNumber { get; set; }
    public string InvoiceNumberFormat { get; set; } = string.Empty;
    public bool IncludeFinancialYearInInvoice { get; set; }
    public bool IsGstEnabled { get; set; }
    public Guid? DefaultTaxId { get; set; }
    public string Currency { get; set; } = "INR";
    public string CurrencySymbol { get; set; } = "₹";
    public string? TermsAndConditions { get; set; }
    public string? InvoiceFooterNote { get; set; }
    public bool ShowLogoOnInvoice { get; set; }
    public bool ShowTaxBreakupOnInvoice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}