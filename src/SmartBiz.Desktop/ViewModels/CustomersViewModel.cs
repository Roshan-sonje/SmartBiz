using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;
using System.Collections.ObjectModel;
using Windows.Services.Maps;

namespace SmartBiz.Desktop.ViewModels;

public partial class CustomersViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly IDialogService _dialog;
    private readonly ILogger<CustomersViewModel> _logger;

    public ObservableCollection<CustomerRow> Customers { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _pageSize = 20;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private string _pageInfo = "Page 1 of 1";

    public CustomersViewModel(
        ICustomerService customerService,
        IDialogService dialog,
        ILogger<CustomersViewModel> logger)
    {
        _customerService = customerService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var result = await _customerService.GetCustomersAsync(
                page: CurrentPage,
                pageSize: PageSize,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                includeInactive: false);

            Customers.Clear();

            foreach (var c in result.Items)
            {
                Customers.Add(new CustomerRow
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone ?? "—",
                    Email = c.Email ?? "—",
                    City = c.City ?? "—",
                    BalanceText = c.OpeningBalance.ToString("N2")
                });
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            IsEmpty = Customers.Count == 0;
            PageInfo = $"Page {CurrentPage} of {Math.Max(1, TotalPages)}  •  {TotalCount} customer{(TotalCount == 1 ? "" : "s")}";

            _logger.LogInformation("Loaded {Count} customers (page {Page}/{Total})",
                Customers.Count, CurrentPage, TotalPages);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load customers");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading customers");
            ErrorMessage = "Failed to load customers.";
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
    private async Task DeleteCustomerAsync(CustomerRow? row)
    {
        if (row is null) return;

        var confirmed = await _dialog.ConfirmAsync(
            "Delete customer",
            $"Delete {row.Name}? This will deactivate the customer.",
            "Delete", "Cancel");

        if (!confirmed) return;

        try
        {
            await _customerService.DeleteCustomerAsync(row.Id);
            await _dialog.ShowAlertAsync("Done", $"{row.Name} has been deleted.");
            await LoadAsync();
        }
        catch (ApiException ex)
        {
            await _dialog.ShowAlertAsync("Error", ex.Message);
        }
    }
}

public class CustomerRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string BalanceText { get; set; } = string.Empty;
}