using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Sales;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class SaleDetailsViewModel : BaseViewModel
{
    private readonly ISaleService _saleService;
    private readonly ILogger<SaleDetailsViewModel> _logger;

    [ObservableProperty] private Sale _sale = new();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _canCancel;

    // Formatted helpers
    public string InvoiceDateText => Sale.InvoiceDate.ToLocalTime().ToString("dd MMM yyyy HH:mm");
    public string SubtotalText => "₹" + Sale.Subtotal.ToString("N2");
    public string DiscountText => "- ₹" + Sale.DiscountAmount.ToString("N2");
    public string TaxText => "₹" + Sale.TaxAmount.ToString("N2");
    public string RoundOffText => Sale.RoundOff.ToString("N2");
    public string GrandTotalText => "₹" + Sale.GrandTotal.ToString("N2");
    public string PaidText => "₹" + Sale.PaidAmount.ToString("N2");
    public string DueText => "₹" + Sale.DueAmount.ToString("N2");

    public SaleDetailsViewModel(ISaleService saleService, ILogger<SaleDetailsViewModel> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    public async Task LoadAsync(Guid saleId)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Sale = await _saleService.GetSaleAsync(saleId);
            CanCancel = Sale.Status != "Cancelled";

            OnPropertyChanged(nameof(InvoiceDateText));
            OnPropertyChanged(nameof(SubtotalText));
            OnPropertyChanged(nameof(DiscountText));
            OnPropertyChanged(nameof(TaxText));
            OnPropertyChanged(nameof(RoundOffText));
            OnPropertyChanged(nameof(GrandTotalText));
            OnPropertyChanged(nameof(PaidText));
            OnPropertyChanged(nameof(DueText));
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load sale");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading sale");
            ErrorMessage = "Failed to load sale.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> CancelAsync()
    {
        try
        {
            await _saleService.CancelSaleAsync(Sale.Id, "Cancelled by user");
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
    }
}