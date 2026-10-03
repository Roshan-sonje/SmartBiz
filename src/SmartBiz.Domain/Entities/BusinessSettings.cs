using SmartBiz.Domain.Common;

namespace SmartBiz.Domain.Entities;

public class BusinessSettings : BaseEntity, IBusinessScoped
{
    public Guid BusinessId { get; set; }

    // Invoice numbering
    public string InvoicePrefix { get; set; } = "INV";
    public long InvoiceNextNumber { get; set; } = 1;
    public string InvoiceNumberFormat { get; set; } = "{PREFIX}-{YEAR}-{NUMBER:000000}";
    public bool IncludeFinancialYearInInvoice { get; set; } = true;

    // Tax
    public bool IsGstEnabled { get; set; } = true;
    public Guid? DefaultTaxId { get; set; }

    // Currency & locale
    public string Currency { get; set; } = "INR";
    public string CurrencySymbol { get; set; } = "₹";
    public string DecimalSeparator { get; set; } = ".";

    // Invoice content
    public string? TermsAndConditions { get; set; }
    public string? InvoiceFooterNote { get; set; }
    public bool ShowLogoOnInvoice { get; set; } = true;
    public bool ShowTaxBreakupOnInvoice { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = null!;
    public Tax? DefaultTax { get; set; }
}