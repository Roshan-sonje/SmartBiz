using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class SalesViewModel : BaseViewModel
{
    private readonly ISaleService _saleService;
    private readonly IDialogService _dialog;
    private readonly ILogger<SalesViewModel> _logger;

    public ObservableCollection<SaleRow> Sales { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _paymentFilter = "All";
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _pageSize = 20;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private string _pageInfo = "Page 1 of 1";

    public List<string> PaymentFilters { get; } = new() { "All", "Unpaid", "Partial", "Paid" };

    public SalesViewModel(ISaleService saleService, IDialogService dialog, ILogger<SalesViewModel> logger)
    {
        _saleService = saleService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var status = PaymentFilter == "All" ? null : PaymentFilter;

            var result = await _saleService.GetSalesAsync(
                page: CurrentPage,
                pageSize: PageSize,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                paymentStatus: status);

            Sales.Clear();
            foreach (var s in result.Items)
            {
                Sales.Add(new SaleRow
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    DateText = s.InvoiceDate.ToLocalTime().ToString("dd-MM-yyyy HH:mm"),
                    CustomerName = s.CustomerName,
                    TotalText = "₹" + s.GrandTotal.ToString("N2"),
                    PaidText = "₹" + s.PaidAmount.ToString("N2"),
                    DueText = "₹" + s.DueAmount.ToString("N2"),
                    PaymentStatus = s.PaymentStatus,
                    PaymentColor = s.PaymentStatus switch
                    {
                        "Paid" => "#059669",
                        "Partial" => "#D97706",
                        "Unpaid" => "#DC2626",
                        _ => "#6B7280"
                    },
                    StatusText = s.Status,
                    ItemCountText = $"{s.ItemCount} item{(s.ItemCount == 1 ? "" : "s")}"
                });
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            IsEmpty = Sales.Count == 0;
            PageInfo = $"Page {CurrentPage} of {Math.Max(1, TotalPages)}  •  {TotalCount} sale{(TotalCount == 1 ? "" : "s")}";
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load sales");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading sales");
            ErrorMessage = "Failed to load sales.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage >= TotalPages) return;
        CurrentPage++;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task PrevPageAsync()
    {
        if (CurrentPage <= 1) return;
        CurrentPage--;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();

    partial void OnPaymentFilterChanged(string value)
    {
        CurrentPage = 1;
        _ = LoadAsync();
    }
}

public class SaleRow
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string DateText { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TotalText { get; set; } = string.Empty;
    public string PaidText { get; set; } = string.Empty;
    public string DueText { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentColor { get; set; } = "#6B7280";
    public string StatusText { get; set; } = string.Empty;
    public string ItemCountText { get; set; } = string.Empty;
}