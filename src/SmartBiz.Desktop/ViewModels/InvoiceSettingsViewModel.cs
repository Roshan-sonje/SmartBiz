using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Business;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class InvoiceSettingsViewModel : BaseViewModel
{
    private readonly IBusinessService _businessService;
    private readonly IDialogService _dialog;
    private readonly ILogger<InvoiceSettingsViewModel> _logger;

    // Form fields
    [ObservableProperty] private string _invoicePrefix = "INV";
    [ObservableProperty] private string _invoiceNumberFormat = "{PREFIX}-{YEAR}-{NUMBER:000000}";
    [ObservableProperty] private bool _includeFinancialYearInInvoice = true;
    [ObservableProperty] private bool _isGstEnabled = true;
    [ObservableProperty] private string _currency = "INR";
    [ObservableProperty] private string _currencySymbol = "₹";
    [ObservableProperty] private bool _showLogoOnInvoice = true;
    [ObservableProperty] private bool _showTaxBreakupOnInvoice = true;
    [ObservableProperty] private string? _termsAndConditions;
    [ObservableProperty] private string? _invoiceFooterNote;

    // State
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;

    public InvoiceSettingsViewModel(
        IBusinessService businessService,
        IDialogService dialog,
        ILogger<InvoiceSettingsViewModel> logger)
    {
        _businessService = businessService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var settings = await _businessService.GetSettingsAsync();

            InvoicePrefix = settings.InvoicePrefix;
            InvoiceNumberFormat = settings.InvoiceNumberFormat;
            IncludeFinancialYearInInvoice = settings.IncludeFinancialYearInInvoice;
            IsGstEnabled = settings.IsGstEnabled;
            Currency = settings.Currency;
            CurrencySymbol = settings.CurrencySymbol;
            ShowLogoOnInvoice = settings.ShowLogoOnInvoice;
            ShowTaxBreakupOnInvoice = settings.ShowTaxBreakupOnInvoice;
            TermsAndConditions = settings.TermsAndConditions;
            InvoiceFooterNote = settings.InvoiceFooterNote;

            _logger.LogInformation("Invoice settings loaded");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load invoice settings");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading settings");
            ErrorMessage = "Failed to load invoice settings.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = null;

        // Basic validation
        if (string.IsNullOrWhiteSpace(InvoicePrefix))
        {
            ErrorMessage = "Invoice prefix is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(InvoiceNumberFormat))
        {
            ErrorMessage = "Invoice number format is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Currency))
        {
            ErrorMessage = "Currency is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrencySymbol))
        {
            ErrorMessage = "Currency symbol is required.";
            return;
        }

        IsSaving = true;
        try
        {
            var request = new UpdateBusinessSettingsRequest
            {
                InvoicePrefix = InvoicePrefix.Trim(),
                InvoiceNumberFormat = InvoiceNumberFormat.Trim(),
                IncludeFinancialYearInInvoice = IncludeFinancialYearInInvoice,
                IsGstEnabled = IsGstEnabled,
                Currency = Currency.Trim(),
                CurrencySymbol = CurrencySymbol.Trim(),
                ShowLogoOnInvoice = ShowLogoOnInvoice,
                ShowTaxBreakupOnInvoice = ShowTaxBreakupOnInvoice,
                TermsAndConditions = string.IsNullOrWhiteSpace(TermsAndConditions) ? null : TermsAndConditions.Trim(),
                InvoiceFooterNote = string.IsNullOrWhiteSpace(InvoiceFooterNote) ? null : InvoiceFooterNote.Trim()
            };

            await _businessService.UpdateSettingsAsync(request);

            await _dialog.ShowAlertAsync("Success", "Invoice settings updated successfully.");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to save invoice settings");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving settings");
            ErrorMessage = "Failed to save changes. Please try again.";
        }
        finally
        {
            IsSaving = false;
        }
    }
}