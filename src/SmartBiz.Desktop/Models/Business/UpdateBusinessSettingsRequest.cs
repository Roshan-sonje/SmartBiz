namespace SmartBiz.Desktop.Models.Business;

public class UpdateBusinessSettingsRequest
{
    public string InvoicePrefix { get; set; } = "INV";
    public string InvoiceNumberFormat { get; set; } = "{PREFIX}-{YEAR}-{NUMBER:000000}";
    public bool IncludeFinancialYearInInvoice { get; set; } = true;
    public bool IsGstEnabled { get; set; } = true;
    public string Currency { get; set; } = "INR";
    public string CurrencySymbol { get; set; } = "₹";
    public string? TermsAndConditions { get; set; }
    public string? InvoiceFooterNote { get; set; }
    public bool ShowLogoOnInvoice { get; set; } = true;
    public bool ShowTaxBreakupOnInvoice { get; set; } = true;
}