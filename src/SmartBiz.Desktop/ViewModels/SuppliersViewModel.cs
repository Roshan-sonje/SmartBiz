using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class SuppliersViewModel : BaseViewModel
{
    private readonly ISupplierService _supplierService;
    private readonly IDialogService _dialog;
    private readonly ILogger<SuppliersViewModel> _logger;

    public ObservableCollection<SupplierRow> Suppliers { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _pageSize = 20;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private string _pageInfo = "Page 1 of 1";

    public SuppliersViewModel(
        ISupplierService supplierService,
        IDialogService dialog,
        ILogger<SuppliersViewModel> logger)
    {
        _supplierService = supplierService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var result = await _supplierService.GetSuppliersAsync(
                page: CurrentPage,
                pageSize: PageSize,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                includeInactive: false);

            Suppliers.Clear();

            foreach (var s in result.Items)
            {
                Suppliers.Add(new SupplierRow
                {
                    Id = s.Id,
                    Name = s.Name,
                    Phone = s.Phone ?? "—",
                    Email = s.Email ?? "—",
                    City = s.City ?? "—",
                    BalanceText = s.OpeningBalance.ToString("N2")
                });
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            IsEmpty = Suppliers.Count == 0;
            PageInfo = $"Page {CurrentPage} of {Math.Max(1, TotalPages)}  •  {TotalCount} supplier{(TotalCount == 1 ? "" : "s")}";

            _logger.LogInformation("Loaded {Count} suppliers", Suppliers.Count);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load suppliers");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading suppliers");
            ErrorMessage = "Failed to load suppliers.";
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
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteSupplierAsync(SupplierRow? row)
    {
        if (row is null) return;

        var confirmed = await _dialog.ConfirmAsync(
            "Delete supplier",
            $"Delete {row.Name}? This will deactivate the supplier.",
            "Delete", "Cancel");

        if (!confirmed) return;

        try
        {
            await _supplierService.DeleteSupplierAsync(row.Id);
            await _dialog.ShowAlertAsync("Done", $"{row.Name} has been deleted.");
            await LoadAsync();
        }
        catch (ApiException ex)
        {
            await _dialog.ShowAlertAsync("Error", ex.Message);
        }
    }
}

public class SupplierRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BalanceText { get; set; } = string.Empty;
}